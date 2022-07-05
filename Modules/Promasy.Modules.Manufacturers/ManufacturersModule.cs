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
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Manufacturers.Dtos;
using Promasy.Modules.Manufacturers.Interfaces;
using Promasy.Modules.Manufacturers.Models;

namespace Promasy.Modules.Manufacturers;

public class ManufacturersModule : IModule
{
    public const string Tag = "Manufacturer";
    public const string RoutePrefix = "/api/manufacturers";

    public IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(RoutePrefix, async (PagedRequest request, [FromServices] IManufacturersRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return Results.Json(list);
            })
            .WithValidator<PagedRequest>()
            .WithTags(Tag)
            .WithName("Get Manufacturers list")
            .RequireAuthorization()
            .Produces<PagedResponse<ManufacturerDto>>();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IManufacturersRepository repository) =>
            {
                var manufacturer = await repository.GetByIdAsync(id);
                return manufacturer is not null ? Results.Json(manufacturer) : Results.NotFound();
            })
            .WithTags(Tag)
            .WithName("Get Manufacturer by Id")
            .RequireAuthorization()
            .Produces<ManufacturerDto>()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateManufacturerRequest request, [FromServices] IManufacturersRepository repository) =>
            {
                var id = await repository.CreateAsync(new ManufacturerDto(0, request.Name));
                var manufacturer = await repository.GetByIdAsync(id);

                return Results.Json(manufacturer, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateManufacturerRequest>()
            .WithTags(Tag)
            .WithName("Create Manufacturer")
            .RequireAuthorization()
            .Produces<ManufacturerDto>(StatusCodes.Status201Created);

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateManufacturerRequest request, [FromRoute] int id, [FromServices] IManufacturersRepository repository) =>
                {
                    if (request.Id != id)
                    {
                        return PromasyResults.ValidationError("Incorrect Id");
                    }

                    await repository.UpdateAsync(new ManufacturerDto(request.Id, request.Name));

                    return Results.Ok(StatusCodes.Status202Accepted);
                })
            .WithValidator<UpdateManufacturerRequest>()
            .WithTags(Tag)
            .WithName("Update Manufacturer")
            .RequireAuthorization()
            .Produces<ManufacturerDto>(StatusCodes.Status202Accepted);

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IManufacturersRepository repository,  
                [FromServices] IManufacturersRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
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
                    return PromasyResults.ValidationError(localizer["Manufacturer already associated with order"],
                        StatusCodes.Status409Conflict);
                }

                await repository.DeleteByIdAsync(id);
                return Results.Ok(StatusCodes.Status204NoContent);
            })
            .WithTags(Tag)
            .WithName("Delete Manufacturer by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationErrorResponse>(StatusCodes.Status409Conflict);
        
        endpoints.MapPost($"{RoutePrefix}/merge", async ([FromBody] MergeManufacturersRequest request, [FromServices] IManufacturersRepository repository) =>
            {
                await repository.MergeAsync(request.TargetId, request.SourceIds);

                return Results.Ok();
            })
            .WithValidator<MergeManufacturersRequest>()
            .WithTags(Tag)
            .WithName("Merge Manufacturers")
            .RequireAuthorization(AdminOnlyPolicy.Name)
            .Produces(StatusCodes.Status200OK);

        return endpoints;
    }
}