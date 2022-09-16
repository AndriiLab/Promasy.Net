using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Policies;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Orders.Dtos;
using Promasy.Modules.Orders.Interfaces;
using Promasy.Modules.Orders.Models;

namespace Promasy.Modules.Orders;

public class ReasonForSupplierChoiceSubModule : SubModule
{
    public override string Tag { get; } = "Reason for Supplier Choice";
    
    public ReasonForSupplierChoiceSubModule(string parentRoutePrefix)
        : base(parentRoutePrefix, "/reasons-for-supplier-choice")
    {
    }


    public override IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        return builder;
    }

    public override IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(RoutePrefix, async (PagedRequest request, [FromServices] IReasonForSupplierChoiceRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return Results.Json(list);
            })
            .WithValidator<PagedRequest>()
            .WithTags(Tag)
            .WithName("Get Reasons for Supplier Choice list")
            .RequireAuthorization()
            .Produces<PagedResponse<ReasonForSupplierChoiceDto>>();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IReasonForSupplierChoiceRepository repository) =>
            {
                var unit = await repository.GetByIdAsync(id);
                return unit is not null ? Results.Json(unit) : Results.NotFound();
            })
            .WithTags(Tag)
            .WithName("Get Reason for Supplier Choice by Id")
            .RequireAuthorization()
            .Produces<ReasonForSupplierChoiceDto>()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateReasonForSupplierChoiceRequest request, [FromServices] IReasonForSupplierChoiceRepository repository) =>
            {
                var id = await repository.CreateAsync(new ReasonForSupplierChoiceDto(0, request.Name));
                var unit = await repository.GetByIdAsync(id);

                return Results.Json(unit, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateReasonForSupplierChoiceRequest>()
            .WithTags(Tag)
            .WithName("Create Reason for Supplier Choice")
            .RequireAuthorization()
            .Produces<ReasonForSupplierChoiceDto>(StatusCodes.Status201Created);

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateReasonForSupplierChoiceRequest request, [FromRoute] int id, [FromServices] IReasonForSupplierChoiceRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.Id != id)
                    {
                        return PromasyResults.ValidationError(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new ReasonForSupplierChoiceDto(request.Id, request.Name));

                    return Results.Ok(StatusCodes.Status202Accepted);
                })
            .WithValidator<UpdateReasonForSupplierChoiceRequest>()
            .WithTags(Tag)
            .WithName("Update Reason for Supplier Choice")
            .RequireAuthorization()
            .Produces(StatusCodes.Status202Accepted);

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IReasonForSupplierChoiceRepository repository,
                [FromServices] IReasonForSupplierChoiceRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
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
                    return PromasyResults.ValidationError(localizer["Reason for Supplier Choice already associated with order"],
                        StatusCodes.Status409Conflict);
                }

                await repository.DeleteByIdAsync(id);
                return Results.Ok(StatusCodes.Status204NoContent);
            })
            .WithTags(Tag)
            .WithName("Delete Reason for Supplier Choice by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationErrorResponse>(StatusCodes.Status409Conflict);
        
        endpoints.MapPost($"{RoutePrefix}/merge", async ([FromBody] MergeReasonForSupplierChoiceRequest request, [FromServices] IReasonForSupplierChoiceRepository repository) =>
            {
                await repository.MergeAsync(request.TargetId, request.SourceIds);

                return Results.Ok();
            })
            .WithValidator<MergeReasonForSupplierChoiceRequest>()
            .WithTags(Tag)
            .WithName("Merge Reasons for Supplier Choice")
            .RequireAuthorization(AdminOnlyPolicy.Name)
            .Produces(StatusCodes.Status200OK);


        return endpoints;
    }
}