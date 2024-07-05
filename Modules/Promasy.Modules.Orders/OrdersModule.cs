using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Domain.Employees;
using Promasy.Modules.Core.Exceptions;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.OpenApi;
using Promasy.Modules.Core.Permissions;
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

    public WebApplication MapEndpoints(WebApplication app)
    {
        app.MapGet(RoutePrefix, async ([AsParameters] OrdersPagedRequest request, [FromServices] IOrdersRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithAuthorizationAndValidation<OrdersPagedRequest>(app, Tag, "Get Orders list", PermissionTag.List,
                Enum.GetValues<RoleName>().Select(r =>
                (r, r switch
                    {
                        RoleName.User => PermissionCondition.SameDepartment,
                        RoleName.HeadOfDepartment => PermissionCondition.SameDepartment,
                        RoleName.PersonallyLiableEmployee => PermissionCondition.SameDepartment,
                        _ => PermissionCondition.None
                    })).ToArray());

        app.MapGet($"{RoutePrefix}/suggestions", async ([AsParameters] OrderSuggestionPagedRequest request, [FromServices] IOrdersRepository repository) =>
            {
                var list = await repository.GetOrderSuggestionsPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<OrderSuggestionPagedRequest>()
            .WithApiDescription(Tag, "GetOrdersSuggestionsList", "Get Orders suggestions list")
            .RequireAuthorization();


        app.MapGet($"{RoutePrefix}/{{id:int}}", async ([AsParameters] GetOrderRequest request, [FromServices] IOrdersRepository repository) =>
            {
                var order = await repository.GetByIdAsync(request.Id);
                if (order is null)
                {
                    throw new ApiException(null, StatusCodes.Status404NotFound);
                }
                return TypedResults.Ok(order);
            })
            .WithAuthorizationAndValidation<GetOrderRequest>(app, Tag, "Get Order by Id", PermissionTag.Get,
                Enum.GetValues<RoleName>().Select(r =>
                (r, r switch
                    {
                        RoleName.User => PermissionCondition.SameDepartment,
                        RoleName.HeadOfDepartment => PermissionCondition.SameDepartment,
                        RoleName.PersonallyLiableEmployee => PermissionCondition.SameDepartment,
                        _ => PermissionCondition.None
                    })).ToArray());

        app.MapPost(RoutePrefix, async ([FromBody]CreateOrderRequest request, [FromServices] IOrdersRepository repository) =>
            {
                var id = await repository.CreateAsync(new CreateOrderDto(request.Description, request.CatNum,
                    request.OnePrice, request.Amount, request.Type, request.Kekv, request.ProcurementStartDate,
                    request.UnitId, request.CpvId, request.FinanceSubDepartmentId, request.ManufacturerId,
                    request.SupplierId, request.ReasonId));
                var order = await repository.GetByIdAsync(id);

                return TypedResults.Json(order, statusCode: StatusCodes.Status201Created);
            })
            .WithAuthorizationAndValidation<CreateOrderRequest>(app, Tag, "Create Order", PermissionTag.Create,
                Enum.GetValues<RoleName>().Select(r =>
                (r, r switch
                    {
                        RoleName.User => PermissionCondition.SameSubDepartment,
                        RoleName.HeadOfDepartment => PermissionCondition.SameDepartment,
                        RoleName.PersonallyLiableEmployee => PermissionCondition.SameDepartment,
                        _ => PermissionCondition.None
                    })).ToArray());

        app.MapPut($"{RoutePrefix}/{{id:int}}",
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
            .WithAuthorizationAndValidation<UpdateOrderRequest>(app, Tag, "Update Order", PermissionTag.Update,
                Enum.GetValues<RoleName>().Select(r =>
                (r, r switch
                    {
                        RoleName.User => PermissionCondition.SameUser,
                        RoleName.HeadOfDepartment => PermissionCondition.SameDepartment,
                        RoleName.PersonallyLiableEmployee => PermissionCondition.SameDepartment,
                        _ => PermissionCondition.None
                    })).ToArray());

        app.MapDelete($"{RoutePrefix}/{{id:int}}", async ([AsParameters] DeleteOrderRequest request, [FromServices] IOrdersRepository repository) =>
            {
                await repository.DeleteByIdAsync(request.Id);
                return TypedResults.NoContent();
            })
            .WithAuthorizationAndValidation<DeleteOrderRequest>(app, Tag, "Delete Order by Id", PermissionTag.Delete,
                Enum.GetValues<RoleName>().Select(r =>
                (r, r switch
                    {
                        RoleName.User => PermissionCondition.SameUser,
                        RoleName.HeadOfDepartment => PermissionCondition.SameDepartment,
                        RoleName.PersonallyLiableEmployee => PermissionCondition.SameDepartment,
                        _ => PermissionCondition.None
                    })).ToArray());
        
        _rfscSubModule.MapEndpoints(app);
        
        app.MapPost($"{RoutePrefix}/export/pdf", async ([FromBody] ExportToPdfRequest request,
                [FromServices] IOrderGroupRepository repository, [FromServices] IOrderExporter exporter) =>
            {

                var fileName = await repository.CreateOrderGroupAsync(request.OrderIds,
                    request.SignEmployees.Select(kv => (kv.Value, kv.Key)),
                    FileType.Pdf);
                
                await exporter.ExportToPdfFileAsync(fileName);
                return TypedResults.Ok(new ExportResponse(fileName));
            })
            .WithAuthorizationAndValidation<ExportToPdfRequest>(app, Tag, "Export as PDF", s => PermissionTag.Export($"{s}/PDF"),
                Enum.GetValues<RoleName>().Select(r =>
                (r, r switch
                    {
                        RoleName.User => PermissionCondition.SameDepartment,
                        RoleName.HeadOfDepartment => PermissionCondition.SameDepartment,
                        RoleName.PersonallyLiableEmployee => PermissionCondition.SameDepartment,
                        _ => PermissionCondition.None
                    })).ToArray());
        
        return app;
    }
}