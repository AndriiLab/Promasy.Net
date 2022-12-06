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

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(RoutePrefix, async ([AsParameters] PagedRequest request, [FromServices] IUnitsRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<PagedRequest>()
            .WithApiDescription(Tag, "GetUnitsList", "Get Units list")
            .RequireAuthorization();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromQuery] int id, [FromServices] IUnitsRepository repository) =>
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

        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateUnitRequest request, [FromServices] IUnitsRepository repository) =>
            {
                var id = await repository.CreateAsync(new UnitDto(0, request.Name));
                var unit = await repository.GetByIdAsync(id);

                return TypedResults.Json(unit, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateUnitRequest>()
            .WithApiDescription(Tag, "CreateUnit", "Create Unit")
            .RequireAuthorization();

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateUnitRequest request, [FromRoute] int id, [FromServices] IUnitsRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.Id != id)
                    {
                        throw new ApiException(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new UnitDto(request.Id, request.Name));

                    return TypedResults.Accepted($"{RoutePrefix}/{request.Id}");
                })
            .WithValidator<UpdateUnitRequest>()
            .WithApiDescription(Tag, "UpdateUnit", "Update Unit")
            .RequireAuthorization();

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async ([FromQuery] int id, [FromServices] IUnitsRepository repository,
                [FromServices] IUnitRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
            {
                var isEditable = await rules.IsEditableAsync(id, CancellationToken.None);
                if (!isEditable)
                {
                    throw new ApiException(localizer["You cannot perform this action"], StatusCodes.Status409Conflict);
                }
                
                var isUsed = await rules.IsUsedAsync(id, CancellationToken.None);
                if (isUsed)
                {
                    throw new ApiException(localizer["Unit already associated with order"],
                        statusCode: StatusCodes.Status409Conflict);
                }

                await repository.DeleteByIdAsync(id);
                return TypedResults.NoContent();
            })
            .WithApiDescription(Tag, "DeleteUnit", "Delete Unit")
            .RequireAuthorization()
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        
        endpoints.MapPost($"{RoutePrefix}/merge", async ([FromBody] MergeUnitsRequest request, [FromServices] IUnitsRepository repository) =>
            {
                await repository.MergeAsync(request.TargetId, request.SourceIds);

                return TypedResults.Ok();
            })
            .WithValidator<MergeUnitsRequest>()
            .WithApiDescription(Tag, "MergeUnits", "Merge Units")
            .RequireAuthorization(AdminOnlyPolicy.Name);


        return endpoints;
    }
}