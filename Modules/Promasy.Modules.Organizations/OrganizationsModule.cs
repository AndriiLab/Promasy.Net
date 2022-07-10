using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Domain.Organizations;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Policies;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Organizations.Dtos;
using Promasy.Modules.Organizations.Interfaces;
using Promasy.Modules.Organizations.Models;

namespace Promasy.Modules.Organizations;

public class OrganizationsModule : IModule
{
    private readonly DepartmentsSubModule _departmentsSubModule;
    public string Tag { get; } = "Organization";
    public string RoutePrefix { get; } = "/api/organizations";

    public OrganizationsModule()
    {
        _departmentsSubModule = new DepartmentsSubModule($"{RoutePrefix}/{{organizationId:int}}");
    }
    
    public IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        _departmentsSubModule.RegisterServices(builder, configuration);
        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{RoutePrefix}/city-types", ([FromServices] IStringLocalizer<CityType> localizer) =>
            {
                return Results.Json(Enum.GetValues<CityType>()
                    .Select(r => new SelectItem<int>((int) r, localizer[r.ToString()])));
            })
            .WithTags(Tag)
            .WithName("Get available city types")
            .Produces<SelectItem<int>[]>();
        
        endpoints.MapGet($"{RoutePrefix}/street-types", ([FromServices] IStringLocalizer<StreetType> localizer) =>
            {
                return Results.Json(Enum.GetValues<StreetType>()
                    .Select(r => new SelectItem<int>((int) r, localizer[r.ToString()])));
            })
            .WithTags(Tag)
            .WithName("Get available street types")
            .Produces<SelectItem<int>[]>();
        
        endpoints.MapGet(RoutePrefix, async (PagedRequest request, [FromServices] IOrganizationsRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return Results.Json(list);
            })
            .WithValidator<PagedRequest>()
            .WithTags(Tag)
            .WithName("Get Organizations list")
            .RequireAuthorization()
            .Produces<PagedResponse<OrganizationShortDto>>();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IOrganizationsRepository repository) =>
            {
                var unit = await repository.GetByIdAsync(id);
                return unit is not null ? Results.Json(unit) : Results.NotFound();
            })
            .WithTags(Tag)
            .WithName("Get Organization by Id")
            .RequireAuthorization()
            .Produces<OrganizationDto>()
            .Produces(StatusCodes.Status404NotFound);

        // endpoints.MapPost(RoutePrefix, async ([FromBody]CreateOrganizationRequest request, [FromServices] IOrganizationsRepository repository) =>
        //     {
        //         var id = await repository.CreateAsync(new OrganizationDto(0, request.Name, request.Email, request.Edrpou,
        //             request.PhoneNumber, request.FaxNumber, request.Country, request.PostalCode, request.Region,
        //             request.City, request.CityType, request.Street, request.StreetType, request.BuildingNumber,
        //             request.InternalNumber));
        //         var unit = await repository.GetByIdAsync(id);
        //
        //         return Results.Json(unit, statusCode: StatusCodes.Status201Created);
        //     })
        //     .WithValidator<CreateOrganizationRequest>()
        //     .WithTags(Tag)
        //     .WithName("Create Organization")
        //     .RequireAuthorization(AdminOnlyPolicy.Name)
        //     .Produces<OrganizationDto>(StatusCodes.Status201Created);

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateOrganizationRequest request, [FromRoute] int id, [FromServices] IOrganizationsRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.Id != id)
                    {
                        return PromasyResults.ValidationError(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new OrganizationDto(request.Id, request.Name, request.Email, request.Edrpou,
                        request.PhoneNumber, request.FaxNumber, request.Country, request.PostalCode, request.Region,
                        request.City, request.CityType, request.Street, request.StreetType, request.BuildingNumber,
                        request.InternalNumber));

                    return Results.Ok(StatusCodes.Status202Accepted);
                })
            .WithValidator<UpdateOrganizationRequest>()
            .WithTags(Tag)
            .WithName("Update Organization")
            .RequireAuthorization()
            .Produces(StatusCodes.Status202Accepted);

        // endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IOrganizationsRepository repository,
        //         [FromServices] IOrganizationsRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
        //     {
        //         var isUsed = await rules.IsUsedAsync(id, CancellationToken.None);
        //         if (isUsed)
        //         {
        //             return PromasyResults.ValidationError(localizer["Organization already has departments"],
        //                 StatusCodes.Status409Conflict);
        //         }
        //
        //         await repository.DeleteByIdAsync(id);
        //         return Results.Ok(StatusCodes.Status204NoContent);
        //     })
        //     .WithTags(Tag)
        //     .WithName("Delete Organization by Id")
        //     .RequireAuthorization(AdminOnlyPolicy.Name)
        //     .Produces(StatusCodes.Status204NoContent)
        //     .Produces<ValidationErrorResponse>(StatusCodes.Status409Conflict);
        
        _departmentsSubModule.MapEndpoints(endpoints);
        
        return endpoints;
    }
}