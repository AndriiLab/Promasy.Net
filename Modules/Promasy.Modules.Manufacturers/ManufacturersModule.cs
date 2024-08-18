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

    public WebApplication MapEndpoints(WebApplication app)
    {
        app.MapGet(RoutePrefix, async ([AsParameters] PagedRequest request, [FromServices] IManufacturersRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<PagedRequest>()
            .WithApiDescription(Tag, "GetManufacturersList", "Get Manufacturers list")
            .RequireAuthorization();

        app.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromServices] IManufacturersRepository repository) =>
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

        app.MapPost(RoutePrefix, async ([FromBody] CreateManufacturerRequest request, [FromServices] IManufacturersRepository repository) =>
            {
                var id = await repository.CreateAsync(new ManufacturerDto(0, request.Name));
                var manufacturer = await repository.GetByIdAsync(id);

                return TypedResults.Json(manufacturer, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateManufacturerRequest>()
            .WithApiDescription(Tag, "CreateManufacturer", "Create Manufacturer")
            .RequireAuthorization();

        app.MapPut($"{RoutePrefix}/{{id:int}}",
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
            .WithAuthorizationAndValidation<UpdateManufacturerRequest>(app, Tag, "Update Manufacturer", PermissionTag.Update,
                Enum.GetValues<RoleName>().Select(r =>
                (r, r switch
                    {
                        RoleName.User => PermissionCondition.SameUser,
                        _ => PermissionCondition.None
                    })).ToArray());

        app.MapDelete($"{RoutePrefix}/{{id:int}}", async ([AsParameters] DeleteManufacturerRequest request, [FromServices] IManufacturersRepository repository,
                [FromServices] IStringLocalizer<SharedResource> localizer) =>
            {
                await repository.DeleteByIdAsync(request.Id);
                return TypedResults.NoContent();
            })
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict)           
            .WithAuthorizationAndValidation<DeleteManufacturerRequest>(app, Tag, "Delete Manufacturer by Id", PermissionTag.Delete,
                Enum.GetValues<RoleName>().Select(r =>
                (r, r switch
                    {
                        RoleName.User => PermissionCondition.SameUser,
                        _ => PermissionCondition.None
                    })).ToArray());

        app.MapPost($"{RoutePrefix}/merge", async ([FromBody] MergeManufacturersRequest request,
                [FromServices] IManufacturersRepository repository) =>
            {
                await repository.MergeAsync(request.TargetId, request.SourceIds);

                return TypedResults.Ok();
            })
            .WithValidator<MergeManufacturersRequest>()
            .WithAuthorization(app, Tag, "Merge Manufacturers", PermissionTag.Merge, RoleName.Administrator);

        return app;
    }
}