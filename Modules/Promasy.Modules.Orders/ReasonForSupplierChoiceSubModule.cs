using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Exceptions;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.OpenApi;
using Promasy.Modules.Core.Policies;
using Promasy.Modules.Core.Requests;
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
        endpoints.MapGet(RoutePrefix, async ([AsParameters] PagedRequest request, [FromServices] IReasonForSupplierChoiceRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<PagedRequest>()
            .WithApiDescription(Tag, "GetReasonsForSupplierChoiceList", "Get Reasons for Supplier Choice list")
            .RequireAuthorization();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromServices] IReasonForSupplierChoiceRepository repository) =>
            {
                var rsc = await repository.GetByIdAsync(id);
                if (rsc is null)
                {
                    throw new ApiException(null, StatusCodes.Status404NotFound);
                }
                return TypedResults.Ok(rsc);
            })
            .WithApiDescription(Tag, "GetReasonsForSupplierChoiceById", "Get Reason for Supplier Choice by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateReasonForSupplierChoiceRequest request, [FromServices] IReasonForSupplierChoiceRepository repository) =>
            {
                var id = await repository.CreateAsync(new ReasonForSupplierChoiceDto(0, request.Name));
                var rsc = await repository.GetByIdAsync(id);

                return TypedResults.Json(rsc, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateReasonForSupplierChoiceRequest>()
            .WithApiDescription(Tag, "CreateReasonsForSupplierChoice", "Create Reason for Supplier Choice")
            .RequireAuthorization();

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateReasonForSupplierChoiceRequest request, [FromRoute] int id, [FromServices] IReasonForSupplierChoiceRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.Id != id)
                    {
                        throw new ApiException(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new ReasonForSupplierChoiceDto(request.Id, request.Name));

                    return TypedResults.Accepted(string.Empty);
                })
            .WithValidator<UpdateReasonForSupplierChoiceRequest>()
            .WithApiDescription(Tag, "UpdateReasonsForSupplierChoice", "Update Reason for Supplier Choice")
            .RequireAuthorization();

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromServices] IReasonForSupplierChoiceRepository repository,
                [FromServices] IReasonForSupplierChoiceRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
            {
                var isEditable = await rules.IsEditableAsync(id, CancellationToken.None);
                if (!isEditable)
                {
                    throw new ApiException(localizer["You cannot perform this action"],
                        statusCode: StatusCodes.Status409Conflict);
                }
                
                var isUsed = await rules.IsUsedAsync(id, CancellationToken.None);
                if (isUsed)
                {
                    throw new ApiException(localizer["Reason for Supplier Choice already associated with order"],
                        statusCode: StatusCodes.Status409Conflict);
                }

                await repository.DeleteByIdAsync(id);
                return TypedResults.NoContent();
            })
            .WithTags(Tag)
            .WithApiDescription(Tag, "DeleteReasonsForSupplierChoiceById", "Delete Reason for Supplier Choice by Id")
            .RequireAuthorization()
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        
        endpoints.MapPost($"{RoutePrefix}/merge", async ([FromBody] MergeReasonForSupplierChoiceRequest request, [FromServices] IReasonForSupplierChoiceRepository repository) =>
            {
                await repository.MergeAsync(request.TargetId, request.SourceIds);

                return TypedResults.Ok();
            })
            .WithValidator<MergeReasonForSupplierChoiceRequest>()
            .WithApiDescription(Tag, "MergeReasonsForSupplierChoice", "Merge Reasons for Supplier Choice")
            .RequireAuthorization(AdminOnlyPolicy.Name);

        return endpoints;
    }
}