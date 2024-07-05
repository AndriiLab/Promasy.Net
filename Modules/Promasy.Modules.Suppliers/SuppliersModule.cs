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
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Suppliers.Dtos;
using Promasy.Modules.Suppliers.Interfaces;
using Promasy.Modules.Suppliers.Models;

namespace Promasy.Modules.Suppliers;

public class SuppliersModule : IModule
{
    public string Tag { get; } = "Supplier";
    public string RoutePrefix { get; } = "/api/suppliers";

    public IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        return builder;
    }

    public WebApplication MapEndpoints(WebApplication app)
    {
        app.MapGet(RoutePrefix, async ([AsParameters] PagedRequest request, [FromServices] ISuppliersRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<PagedRequest>()
            .WithApiDescription(Tag, "GetSuppliersList", "Get Suppliers list")
            .RequireAuthorization();

        app.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromServices] ISuppliersRepository repository) =>
            {
                var supplier = await repository.GetByIdAsync(id);
                if (supplier is null)
                {
                    throw new ApiException(null, StatusCodes.Status404NotFound);
                }

                return TypedResults.Ok(supplier);
            })
            .WithApiDescription(Tag, "GetSupplierById", "Get Supplier by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost(RoutePrefix, async ([FromBody] CreateSupplierRequest request, [FromServices] ISuppliersRepository repository) =>
            {
                var id = await repository.CreateAsync(new SupplierDto(0, request.Name, request.Comment, request.Phone));
                var unit = await repository.GetByIdAsync(id);

                return TypedResults.Json(unit, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateSupplierRequest>()
            .WithApiDescription(Tag, "CreateSupplier", "Create Supplier")
            .RequireAuthorization();

        app.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateSupplierRequest request, [FromRoute] int id, [FromServices] ISuppliersRepository repository, 
        [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.Id != id)
                    {
                        throw new ApiException(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new SupplierDto(request.Id, request.Name, request.Comment, request.Phone));

                    return TypedResults.Accepted($"{RoutePrefix}/{id}");
                })
            .WithAuthorizationAndValidation<UpdateSupplierRequest>(app, Tag, "Update Supplier", PermissionTag.Update,
                Enum.GetValues<RoleName>().Select(r => (r, r == RoleName.User ? PermissionCondition.SameUser : PermissionCondition.None)).ToArray());

        app.MapDelete($"{RoutePrefix}/{{id:int}}", async ([AsParameters] DeleteSupplierRequest model, [FromServices] ISuppliersRepository repository,
                [FromServices] ISupplierRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
            {
                var isEditable = await rules.IsEditableAsync(model.Id, CancellationToken.None);
                if (!isEditable)
                {
                    throw new ApiException(localizer["You cannot perform this action"],
                        statusCode: StatusCodes.Status409Conflict);
                }
                
                var isUsed = await rules.IsUsedAsync(model.Id, CancellationToken.None);
                if (isUsed)
                {
                    throw new ApiException(localizer["Supplier already associated with order"],
                        statusCode: StatusCodes.Status409Conflict);
                }

                await repository.DeleteByIdAsync(model.Id);
                return TypedResults.NoContent();
            })
            .WithAuthorizationAndValidation<DeleteSupplierRequest>(app, Tag, "Delete Supplier by Id", PermissionTag.Delete,
                Enum.GetValues<RoleName>().Select(r => (r, r == RoleName.User ? PermissionCondition.SameUser : PermissionCondition.None)).ToArray())
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        
        app.MapPost($"{RoutePrefix}/merge", async ([FromBody] MergeSuppliersRequest request, [FromServices] ISuppliersRepository repository) =>
            {
                await repository.MergeAsync(request.TargetId, request.SourceIds);

                return TypedResults.Ok();
            })
            .WithValidator<MergeSuppliersRequest>()
            .WithAuthorization(app, Tag, "Merge Suppliers", PermissionTag.Merge, RoleName.Administrator);

        return app;
    }
}