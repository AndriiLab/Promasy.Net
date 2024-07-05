using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Domain.Employees;
using Promasy.Modules.Core.Exceptions;
using Promasy.Modules.Core.Mapper;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.OpenApi;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Units.Dtos;
using Promasy.Modules.Units.Interfaces;
using Promasy.Modules.Units.Models;

namespace Promasy.Modules.Units;

public class UnitsModule : IModule
{
    public string Tag { get; } = "Unit";
    public string RoutePrefix { get; } = "/api/units";

    public IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        return builder;
    }

    public WebApplication MapEndpoints(WebApplication app)
    {
        app.MapGet(RoutePrefix, async ([AsParameters] PagedRequest request, [FromServices] IUnitsRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<PagedRequest>()
            .WithApiDescription(Tag, "GetUnitsList", "Get Units list")
            .RequireAuthorization();

        app.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromQuery] int id, [FromServices] IUnitsRepository repository) =>
            {
                var unit = await repository.GetByIdAsync(id);
                if (unit is null)
                {
                    throw new ApiException(null, StatusCodes.Status404NotFound);
                }
                return TypedResults.Ok(unit);
            })
            .WithApiDescription(Tag, "GetUnitById", "Get Unit by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status404NotFound);

        app.MapPost(RoutePrefix, async ([FromBody]CreateUnitRequest request, [FromServices] IUnitsRepository repository,
            [FromServices] IMapper<CreateUnitRequest, CreateUnitDto> mapper) =>
            {
                var id = await repository.CreateAsync(mapper.MapFromSource(request));
                var unit = await repository.GetByIdAsync(id);

                return TypedResults.Json(unit, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateUnitRequest>()
            .WithApiDescription(Tag, "CreateUnit", "Create Unit")
            .RequireAuthorization();

        app.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateUnitRequest request, [FromRoute] int id, [FromServices] IUnitsRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer, [FromServices] IMapper<UpdateUnitRequest, UpdateUnitDto> mapper) =>
                {
                    if (request.Id != id)
                    {
                        throw new ApiException(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(mapper.MapFromSource(request));

                    return TypedResults.Accepted($"{RoutePrefix}/{request.Id}");
                })
            .WithAuthorizationAndValidation<UpdateUnitRequest>(app, Tag, "Update Unit", PermissionTag.Update, 
                Enum.GetValues<RoleName>().Select(r => (r, r == RoleName.User ? PermissionCondition.SameUser : PermissionCondition.None)).ToArray());

        app.MapDelete($"{RoutePrefix}/{{id:int}}", async ([AsParameters] DeleteUnitRequest model, [FromServices] IUnitsRepository repository,
                [FromServices] IUnitRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
            {
                var isEditable = await rules.IsEditableAsync(model.Id, CancellationToken.None);
                if (!isEditable)
                {
                    throw new ApiException(localizer["You cannot perform this action"], StatusCodes.Status409Conflict);
                }
                
                var isUsed = await rules.IsUsedAsync(model.Id, CancellationToken.None);
                if (isUsed)
                {
                    throw new ApiException(localizer["Unit already associated with order"],
                        statusCode: StatusCodes.Status409Conflict);
                }

                await repository.DeleteByIdAsync(model.Id);
                return TypedResults.NoContent();
            })
            .WithAuthorizationAndValidation<DeleteUnitRequest>(app, Tag, "Delete Unit", PermissionTag.Delete, 
                Enum.GetValues<RoleName>().Select(r => (r, r == RoleName.User ? PermissionCondition.SameUser : PermissionCondition.None)).ToArray())
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        app.MapPost($"{RoutePrefix}/merge",
                async ([FromBody] MergeUnitsRequest request, [FromServices] IUnitsRepository repository) =>
                {
                    await repository.MergeAsync(request.TargetId, request.SourceIds);

                    return TypedResults.Ok();
                })
            .WithValidator<MergeUnitsRequest>()
            .WithAuthorization(app, Tag, "Merge Units", PermissionTag.Merge, RoleName.Administrator);

        return app;
    }
}