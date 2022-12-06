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
using Promasy.Modules.Manufacturers.Dtos;
using Promasy.Modules.Manufacturers.Interfaces;
using Promasy.Modules.Manufacturers.Models;

namespace Promasy.Modules.Manufacturers;

public class ManufacturersModule : IModule
{
    public string Tag { get; } = "Manufacturer";
    public string RoutePrefix { get; } = "/api/manufacturers";

    public IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(RoutePrefix, async ([AsParameters] PagedRequest request, [FromServices] IManufacturersRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<PagedRequest>()
            .WithApiDescription(Tag, "GetManufacturersList", "Get Manufacturers list")
            .RequireAuthorization();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromServices] IManufacturersRepository repository) =>
            {
                var manufacturer = await repository.GetByIdAsync(id);
                if (manufacturer is null)
                {
                    throw new ApiException(null, StatusCodes.Status404NotFound);
                }
                return TypedResults.Ok(manufacturer);
            })
            .WithApiDescription(Tag, "GetManufacturerById", "Get Manufacturer by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(RoutePrefix, async ([FromBody] CreateManufacturerRequest request, [FromServices] IManufacturersRepository repository) =>
            {
                var id = await repository.CreateAsync(new ManufacturerDto(0, request.Name));
                var manufacturer = await repository.GetByIdAsync(id);

                return TypedResults.Json(manufacturer, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateManufacturerRequest>()
            .WithApiDescription(Tag, "CreateManufacturer", "Create Manufacturer")
            .RequireAuthorization();

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateManufacturerRequest request, [FromRoute] int id, [FromServices] IManufacturersRepository repository,
                    [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.Id != id)
                    {
                        throw new ApiException(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new ManufacturerDto(request.Id, request.Name));

                    return TypedResults.Accepted($"{RoutePrefix}/{request.Id}");
                })
            .WithValidator<UpdateManufacturerRequest>()
            .WithApiDescription(Tag, "UpdateManufacturer", "Update Manufacturer")
            .RequireAuthorization();

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromServices] IManufacturersRepository repository,  
                [FromServices] IManufacturerRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
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
                    throw new ApiException(localizer["Manufacturer already associated with order"],
                        statusCode: StatusCodes.Status409Conflict);
                }

                await repository.DeleteByIdAsync(id);
                return TypedResults.NoContent();
            })
            .WithApiDescription(Tag, "DeleteManufacturerById", "Delete Manufacturer by Id")
            .RequireAuthorization()
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        
        endpoints.MapPost($"{RoutePrefix}/merge", async ([FromBody] MergeManufacturersRequest request, [FromServices] IManufacturersRepository repository) =>
            {
                await repository.MergeAsync(request.TargetId, request.SourceIds);

                return TypedResults.Ok();
            })
            .WithValidator<MergeManufacturersRequest>()
            .WithApiDescription(Tag, "MergeManufacturers", "Merge Manufacturers")
            .RequireAuthorization(AdminOnlyPolicy.Name);

        return endpoints;
    }
}