using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Domain.Employees;
using Promasy.Domain.Orders;
using Promasy.Modules.Core.Exceptions;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.OpenApi;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Orders.Dtos;
using Promasy.Modules.Orders.Interfaces;
using Promasy.Modules.Orders.Models;
using Promasy.Modules.Orders.Repositories;
using Promasy.Modules.Orders.Services;

namespace Promasy.Modules.Orders;

public class OrdersModule : IModule
{
    private readonly ReasonForSupplierChoiceSubModule _rfscSubModule;
    public string Tag { get; } = "Order";
    public string RoutePrefix { get; } = "/api/orders";
    
    public OrdersModule()
    {
        _rfscSubModule = new ReasonForSupplierChoiceSubModule(RoutePrefix);
    }
    
    public IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        builder.AddTransient<IOrderExporter, OrderExporter>();
        builder.AddTransient<IEmployeeRules, EmployeeRepository>();
        builder.AddTransient<IEmployeesRepository, EmployeeRepository>();

        _rfscSubModule.RegisterServices(builder, configuration);
        
        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{RoutePrefix}/all-types", ([FromServices] IStringLocalizer<OrderType> localizer) =>
            {
                return TypedResults.Ok(Enum.GetValues<OrderType>()
                    .Select(r => new SelectItem<int>((int) r, localizer[r.ToString()])));
            })
            .WithApiDescription(Tag, "GetAvailableOrderTypes", "Get available order types");
        
        endpoints.MapGet($"{RoutePrefix}/all-statuses", ([FromServices] IStringLocalizer<OrderStatus> localizer) =>
            {
                return TypedResults.Ok(Enum.GetValues<OrderStatus>()
                    .Select(r => new SelectItem<int>((int) r, localizer[r.ToString()])));
            })
            .WithApiDescription(Tag, "GetAvailableOrderStatuses", "Get available order statuses");
        
        endpoints.MapGet(RoutePrefix, async ([AsParameters] OrdersPagedRequest request, [FromServices] IOrdersRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<OrdersPagedRequest>()
            .WithApiDescription(Tag, "GetOrdersList", "Get Orders list")
            .RequireAuthorization();

        endpoints.MapGet($"{RoutePrefix}/suggestions", async ([AsParameters] OrderSuggestionPagedRequest request, [FromServices] IOrdersRepository repository) =>
            {
                var list = await repository.GetOrderSuggestionsPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<OrderSuggestionPagedRequest>()
            .WithApiDescription(Tag, "GetOrdersSuggestionsList", "Get Orders suggestions list")
            .RequireAuthorization();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromServices] IOrdersRepository repository) =>
            {
                var order = await repository.GetByIdAsync(id);
                if (order is null)
                {
                    throw new ApiException(null, StatusCodes.Status404NotFound);
                }
                return TypedResults.Ok(order);
            })
            .WithApiDescription(Tag, "GetOrderById", "Get Order by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateOrderRequest request, [FromServices] IOrdersRepository repository) =>
            {
                var id = await repository.CreateAsync(new CreateOrderDto(request.Description, request.CatNum,
                    request.OnePrice, request.Amount, request.Type, request.Kekv, request.ProcurementStartDate,
                    request.UnitId, request.CpvId, request.FinanceSubDepartmentId, request.ManufacturerId,
                    request.SupplierId, request.ReasonId));
                var order = await repository.GetByIdAsync(id);

                return TypedResults.Json(order, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateOrderRequest>()
            .WithApiDescription(Tag, "CreateOrder", "Create Order")
            .RequireAuthorization();

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateOrderRequest request, [FromRoute] int id, [FromServices] IOrdersRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.Id != id)
                    {
                        throw new ApiException(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new UpdateOrderDto(request.Id, request.Description, request.CatNum,
                        request.OnePrice, request.Amount, request.Type, request.Kekv, request.ProcurementStartDate,
                        request.UnitId, request.CpvId, request.FinanceSubDepartmentId, request.ManufacturerId,
                        request.SupplierId, request.ReasonId));

                    return TypedResults.Accepted(string.Empty);
                })
            .WithValidator<UpdateOrderRequest>()
            .WithApiDescription(Tag, "UpdateOrder", "Update Order")
            .RequireAuthorization();

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IOrdersRepository repository) =>
            {
                await repository.DeleteByIdAsync(id);
                return TypedResults.NoContent();
            })
            .WithApiDescription(Tag, "DeleteOrderById", "Delete Order by Id")
            .RequireAuthorization();
        
        _rfscSubModule.MapEndpoints(endpoints);
        
        endpoints.MapPost($"{RoutePrefix}/export/pdf", async ([FromBody] ExportToPdfRequest request,
                [FromServices] IOrderGroupRepository repository, [FromServices] IOrderExporter exporter) =>
            {

                var fileName = await repository.CreateOrderGroupAsync(request.OrderIds,
                    request.SignEmployees.Select(kv => new Tuple<int, RoleName>(kv.Value, kv.Key)),
                    FileType.Pdf);
                
                await exporter.ExportToPdfFileAsync(fileName);
                return TypedResults.Ok(new ExportResponse(fileName));
            })
        .WithValidator<ExportToPdfRequest>()
        .WithApiDescription(Tag, "ExportAsPdf", "Export as PDF")
        .RequireAuthorization();
        
        return endpoints;
    }
}