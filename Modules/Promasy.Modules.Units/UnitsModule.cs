using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
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
                var list = await repository.GetUnitsAsync(request);
                return Results.Json(list);
            })
            .WithValidator<PagedRequest>()
            .WithTags(Tag)
            .WithName("Get Units list")
            .RequireAuthorization()
            .Produces<PagedResponse<UnitDto>>();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IUnitsRepository repository) =>
            {
                var unit = await repository.GetUnitByIdAsync(id);
                return unit is not null ? Results.Json(unit) : Results.NotFound();
            })
            .WithTags(Tag)
            .WithName("Get Unit by Id")
            .RequireAuthorization()
            .Produces<UnitDto>()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateUnitRequest request, [FromServices] IUnitsRepository repository) =>
            {
                var id = await repository.CreateUnitAsync(new UnitDto(0, request.Name));
                var unit = await repository.GetUnitByIdAsync(id);

                return Results.Json(unit, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateUnitRequest>()
            .WithTags(Tag)
            .WithName("Create Unit")
            .RequireAuthorization()
            .Produces<UnitDto>(StatusCodes.Status201Created);

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateUnitRequest request, [FromRoute] int id, [FromServices] IUnitsRepository repository) =>
                {
                    if (request.Id != id)
                    {
                        return PromasyResults.ValidationError("Incorrect Id");
                    }

                    await repository.UpdateUnitAsync(new UnitDto(request.Id, request.Name));

                    return Results.Ok(StatusCodes.Status202Accepted);
                })
            .WithTags(Tag)
            .WithName("Update Unit")
            .RequireAuthorization()
            .Produces<UnitDto>(StatusCodes.Status202Accepted);

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IUnitsRepository repository,  [FromServices] IUnitsRules rules) =>
            {
                var isEditable = await rules.IsEditableAsync(id, CancellationToken.None);
                if (!isEditable)
                {
                    return PromasyResults.ValidationError("You cannot delete this unit",
                        StatusCodes.Status409Conflict);
                }
                
                var isUsed = await rules.IsUnitUsedAsync(id, CancellationToken.None);
                if (isUsed)
                {
                    return PromasyResults.ValidationError("Unit already associated with order",
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

        return endpoints;
    }
}