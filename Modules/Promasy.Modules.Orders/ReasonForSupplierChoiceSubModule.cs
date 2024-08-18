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

    public override WebApplication MapEndpoints(WebApplication app)
    {
        app.MapGet(RoutePrefix, async ([AsParameters] PagedRequest request, [FromServices] IReasonForSupplierChoiceRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<PagedRequest>()
            .WithApiDescription(Tag, "GetReasonsForSupplierChoiceList", "Get Reasons for Supplier Choice list")
            .RequireAuthorization();

        app.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromServices] IReasonForSupplierChoiceRepository repository) =>
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

        app.MapPost(RoutePrefix, async ([FromBody]CreateReasonForSupplierChoiceRequest request, [FromServices] IReasonForSupplierChoiceRepository repository) =>
            {
                var id = await repository.CreateAsync(new ReasonForSupplierChoiceDto(0, request.Name));
                var rsc = await repository.GetByIdAsync(id);

                return TypedResults.Json(rsc, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateReasonForSupplierChoiceRequest>()
            .WithApiDescription(Tag, "CreateReasonsForSupplierChoice", "Create Reason for Supplier Choice")
            .RequireAuthorization();

        app.MapPut($"{RoutePrefix}/{{id:int}}",
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
            .WithAuthorizationAndValidation<UpdateReasonForSupplierChoiceRequest>(app, Tag, "Update Reason for Supplier Choice", PermissionTag.Update,
                Enum.GetValues<RoleName>().Select(r =>
                (r, r switch
                    {
                        RoleName.User => PermissionCondition.SameUser,
                        _ => PermissionCondition.Role
                    })).ToArray());

        app.MapDelete($"{RoutePrefix}/{{id:int}}", async ([AsParameters] DeleteReasonForSupplierChoiceRequest request, [FromServices] IReasonForSupplierChoiceRepository repository) =>
            {
                await repository.DeleteByIdAsync(request.Id);
                return TypedResults.NoContent();
            })
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
            .WithAuthorizationAndValidation<DeleteReasonForSupplierChoiceRequest>(app, Tag, "Delete Reason for Supplier Choice by Id", PermissionTag.Delete,
                Enum.GetValues<RoleName>().Select(r =>
                (r, r switch
                    {
                        RoleName.User => PermissionCondition.SameUser,
                        _ => PermissionCondition.Role
                    })).ToArray());
            
        app.MapPost($"{RoutePrefix}/merge", async ([FromBody] MergeReasonForSupplierChoiceRequest request, [FromServices] IReasonForSupplierChoiceRepository repository) =>
            {
                await repository.MergeAsync(request.TargetId, request.SourceIds);

                return TypedResults.Ok();
            })
            .WithValidator<MergeReasonForSupplierChoiceRequest>()
            .WithAuthorization(app, Tag, "Merge Reasons for Supplier Choice", PermissionTag.Merge, RoleName.Administrator);

        return app;
    }
}