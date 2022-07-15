using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Policies;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Core.Rules;
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

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(RoutePrefix, async (PagedRequest request, [FromServices] ISuppliersRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return Results.Json(list);
            })
            .WithValidator<PagedRequest>()
            .WithTags(Tag)
            .WithName("Get Suppliers list")
            .RequireAuthorization()
            .Produces<PagedResponse<SupplierDto>>();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] ISuppliersRepository repository) =>
            {
                var unit = await repository.GetByIdAsync(id);
                return unit is not null ? Results.Json(unit) : Results.NotFound();
            })
            .WithTags(Tag)
            .WithName("Get Supplier by Id")
            .RequireAuthorization()
            .Produces<SupplierDto>()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateSupplierRequest request, [FromServices] ISuppliersRepository repository) =>
            {
                var id = await repository.CreateAsync(new SupplierDto(0, request.Name, request.Comment, request.Phone));
                var unit = await repository.GetByIdAsync(id);

                return Results.Json(unit, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateSupplierRequest>()
            .WithTags(Tag)
            .WithName("Create Supplier")
            .RequireAuthorization()
            .Produces<SupplierDto>(StatusCodes.Status201Created);

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateSupplierRequest request, [FromRoute] int id, [FromServices] ISuppliersRepository repository, 
        [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.Id != id)
                    {
                        return PromasyResults.ValidationError(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new SupplierDto(request.Id, request.Name, request.Comment, request.Phone));

                    return Results.Ok(StatusCodes.Status202Accepted);
                })
            .WithValidator<UpdateSupplierRequest>()
            .WithTags(Tag)
            .WithName("Update Supplier")
            .RequireAuthorization()
            .Produces<SupplierDto>(StatusCodes.Status202Accepted);

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] ISuppliersRepository repository,
                [FromServices] ISupplierRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
            {
                var isEditable = await rules.IsEditableAsync(id, CancellationToken.None);
                if (!isEditable)
                {
                    return PromasyResults.ValidationError(localizer["You cannot perform this action"],
                        StatusCodes.Status409Conflict);
                }
                
                var isUsed = await rules.IsUsedAsync(id, CancellationToken.None);
                if (isUsed)
                {
                    return PromasyResults.ValidationError(localizer["Supplier already associated with order"],
                        StatusCodes.Status409Conflict);
                }

                await repository.DeleteByIdAsync(id);
                return Results.Ok(StatusCodes.Status204NoContent);
            })
            .WithTags(Tag)
            .WithName("Delete Supplier by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationErrorResponse>(StatusCodes.Status409Conflict);
        
        endpoints.MapPost($"{RoutePrefix}/merge", async ([FromBody] MergeSuppliersRequest request, [FromServices] ISuppliersRepository repository) =>
            {
                await repository.MergeAsync(request.TargetId, request.SourceIds);

                return Results.Ok();
            })
            .WithValidator<MergeSuppliersRequest>()
            .WithTags(Tag)
            .WithName("Merge Suppliers")
            .RequireAuthorization(AdminOnlyPolicy.Name)
            .Produces(StatusCodes.Status200OK);


        return endpoints;
    }
}