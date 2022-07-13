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
using Promasy.Modules.Core.Rules;
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
        endpoints.MapGet(RoutePrefix, async (PagedRequest request, [FromServices] IUnitsRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return Results.Json(list);
            })
            .WithValidator<PagedRequest>()
            .WithTags(Tag)
            .WithName("Get Units list")
            .RequireAuthorization()
            .Produces<PagedResponse<UnitDto>>();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IUnitsRepository repository) =>
            {
                var unit = await repository.GetByIdAsync(id);
                return unit is not null ? Results.Json(unit) : Results.NotFound();
            })
            .WithTags(Tag)
            .WithName("Get Unit by Id")
            .RequireAuthorization()
            .Produces<UnitDto>()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateUnitRequest request, [FromServices] IUnitsRepository repository) =>
            {
                var id = await repository.CreateAsync(new UnitDto(0, request.Name));
                var unit = await repository.GetByIdAsync(id);

                return Results.Json(unit, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateUnitRequest>()
            .WithTags(Tag)
            .WithName("Create Unit")
            .RequireAuthorization()
            .Produces<UnitDto>(StatusCodes.Status201Created);

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateUnitRequest request, [FromRoute] int id, [FromServices] IUnitsRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.Id != id)
                    {
                        return PromasyResults.ValidationError(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new UnitDto(request.Id, request.Name));

                    return Results.Ok(StatusCodes.Status202Accepted);
                })
            .WithValidator<UpdateUnitRequest>()
            .WithTags(Tag)
            .WithName("Update Unit")
            .RequireAuthorization()
            .Produces<UnitDto>(StatusCodes.Status202Accepted);

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IUnitsRepository repository,
                [FromServices] IUnitsRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
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
                    return PromasyResults.ValidationError(localizer["Unit already associated with order"],
                        StatusCodes.Status409Conflict);
                }

                await repository.DeleteByIdAsync(id);
                return Results.Ok(StatusCodes.Status204NoContent);
            })
            .WithTags(Tag)
            .WithName("Delete Unit by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationErrorResponse>(StatusCodes.Status409Conflict);
        
        endpoints.MapPost($"{RoutePrefix}/merge", async ([FromBody] MergeUnitsRequest request, [FromServices] IUnitsRepository repository) =>
            {
                await repository.MergeAsync(request.TargetId, request.SourceIds);

                return Results.Ok();
            })
            .WithValidator<MergeUnitsRequest>()
            .WithTags(Tag)
            .WithName("Merge Units")
            .RequireAuthorization(AdminOnlyPolicy.Name)
            .Produces(StatusCodes.Status200OK);


        return endpoints;
    }
}