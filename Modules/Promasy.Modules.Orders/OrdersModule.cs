using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Domain.Orders;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Orders.Dtos;
using Promasy.Modules.Orders.Interfaces;
using Promasy.Modules.Orders.Models;

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
        _rfscSubModule.RegisterServices(builder, configuration);
        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{RoutePrefix}/all-types", ([FromServices] IStringLocalizer<OrderType> localizer) =>
            {
                return Results.Json(Enum.GetValues<OrderType>()
                    .Select(r => new SelectItem<int>((int) r, localizer[r.ToString()])));
            })
            .WithTags(Tag)
            .WithName("Get available order types")
            .Produces<SelectItem<int>[]>();
        
        endpoints.MapGet($"{RoutePrefix}/all-statuses", ([FromServices] IStringLocalizer<OrderStatus> localizer) =>
            {
                return Results.Json(Enum.GetValues<OrderStatus>()
                    .Select(r => new SelectItem<int>((int) r, localizer[r.ToString()])));
            })
            .WithTags(Tag)
            .WithName("Get available order statuses")
            .Produces<SelectItem<int>[]>();
        
        endpoints.MapGet(RoutePrefix, async (OrdersPagedRequest request, [FromServices] IOrdersRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return Results.Json(list);
            })
            .WithValidator<OrdersPagedRequest>()
            .WithTags(Tag)
            .WithName("Get Orders list")
            .RequireAuthorization()
            .Produces<OrderPagedResponse>();
        endpoints.MapGet($"{RoutePrefix}/suggestions", async (OrderSuggestionPagedRequest request, [FromServices] IOrdersRepository repository) =>
            {
                var list = await repository.GetOrderSuggestionsPagedListAsync(request);
                return Results.Json(list);
            })
            .WithValidator<OrderSuggestionPagedRequest>()
            .WithTags(Tag)
            .WithName("Get Orders suggestions list")
            .RequireAuthorization()
            .Produces<PagedResponse<OrderSuggestionDto>>();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IOrdersRepository repository) =>
            {
                var order = await repository.GetByIdAsync(id);
                return order is not null ? Results.Json(order) : Results.NotFound();
            })
            .WithTags(Tag)
            .WithName("Get Order by Id")
            .RequireAuthorization()
            .Produces<OrderDto>()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateOrderRequest request, [FromServices] IOrdersRepository repository) =>
            {
                var id = await repository.CreateAsync(new CreateOrderDto(request.Description, request.CatNum,
                    request.OnePrice, request.Amount, request.Type, request.Kekv, request.ProcurementStartDate,
                    request.UnitId, request.CpvId, request.FinanceSubDepartmentId, request.ManufacturerId,
                    request.SupplierId, request.ReasonId));
                var unit = await repository.GetByIdAsync(id);

                return Results.Json(unit, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateOrderRequest>()
            .WithTags(Tag)
            .WithName("Create Order")
            .RequireAuthorization()
            .Produces<OrderDto>(StatusCodes.Status201Created);

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateOrderRequest request, [FromRoute] int id, [FromServices] IOrdersRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.Id != id)
                    {
                        return PromasyResults.ValidationError(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new UpdateOrderDto(request.Id, request.Description, request.CatNum,
                        request.OnePrice, request.Amount, request.Type, request.Kekv, request.ProcurementStartDate,
                        request.UnitId, request.CpvId, request.FinanceSubDepartmentId, request.ManufacturerId,
                        request.SupplierId, request.ReasonId));

                    return Results.Ok(StatusCodes.Status202Accepted);
                })
            .WithValidator<UpdateOrderRequest>()
            .WithTags(Tag)
            .WithName("Update Order")
            .RequireAuthorization()
            .Produces(StatusCodes.Status202Accepted);

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IOrdersRepository repository) =>
            {
                await repository.DeleteByIdAsync(id);
                return Results.Ok(StatusCodes.Status204NoContent);
            })
            .WithTags(Tag)
            .WithName("Delete Order by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationErrorResponse>(StatusCodes.Status409Conflict);
        
        _rfscSubModule.MapEndpoints(endpoints);
        
        return endpoints;
    }
}