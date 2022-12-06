using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Domain.Organizations;
using Promasy.Modules.Core.Exceptions;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.OpenApi;
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
        endpoints.MapGet($"{RoutePrefix}/all-city-types", ([FromServices] IStringLocalizer<CityType> localizer) =>
            {
                return TypedResults.Ok(Enum.GetValues<CityType>()
                    .Select(r => new SelectItem<int>((int) r, localizer[r.ToString()])));
            })
            .WithApiDescription(Tag, "GetAvailableCityTypes", "Get available city types");
        
        endpoints.MapGet($"{RoutePrefix}/all-street-types", ([FromServices] IStringLocalizer<StreetType> localizer) =>
            {
                return TypedResults.Ok(Enum.GetValues<StreetType>()
                    .Select(r => new SelectItem<int>((int) r, localizer[r.ToString()])));
            })
            .WithApiDescription(Tag, "GetAvailableStreetTypes", "Get available street types");
        
        endpoints.MapGet(RoutePrefix, async ([AsParameters] PagedRequest request, [FromServices] IOrganizationsRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<PagedRequest>()
            .WithApiDescription(Tag, "GetOrganizationsLst", "Get Organizations list")
            .RequireAuthorization();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromQuery] int id, [FromServices] IOrganizationsRepository repository) =>
            {
                var organization = await repository.GetByIdAsync(id);
                if (organization is null)
                {
                    throw new ApiException(null, StatusCodes.Status404NotFound);
                }
                return  TypedResults.Ok(organization);
            })
            .WithApiDescription(Tag, "GetOrganizationById", "Get Organization by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status404NotFound);

        // endpoints.MapPost(RoutePrefix, async ([FromBody] CreateOrganizationRequest request, [FromServices] IOrganizationsRepository repository) =>
        //     {
        //         var id = await repository.CreateAsync(new OrganizationDto(0, request.Name, request.Email, request.Edrpou,
        //             request.PhoneNumber, request.FaxNumber, request.Country, request.PostalCode, request.Region,
        //             request.City, request.CityType, request.Street, request.StreetType, request.BuildingNumber,
        //             request.InternalNumber));
        //         var organization = await repository.GetByIdAsync(id);
        //
        //         return TypedResults.Json(organization, statusCode: StatusCodes.Status201Created);
        //     })
        //     .WithValidator<CreateOrganizationRequest>()
        //     .WithApiDescription(Tag, "CreateOrganization", "Create Organization")
        //     .RequireAuthorization(AdminOnlyPolicy.Name);

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateOrganizationRequest request, [FromRoute] int id, [FromServices] IOrganizationsRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.Id != id)
                    {
                        throw new ApiException(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new OrganizationDto(request.Id, request.Name, request.Email, request.Edrpou,
                        request.PhoneNumber, request.FaxNumber, request.Country, request.PostalCode, request.Region,
                        request.City, request.CityType, request.Street, request.StreetType, request.BuildingNumber,
                        request.InternalNumber));

                    return TypedResults.Accepted($"{RoutePrefix}/{id}");
                })
            .WithValidator<UpdateOrganizationRequest>()
            .WithApiDescription(Tag, "UpdateOrganization", "Update Organization")
            .RequireAuthorization();

        // endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromServices] IOrganizationsRepository repository,
        //         [FromServices] IOrganizationsRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
        //     {
        //         var isUsed = await rules.IsUsedAsync(id, CancellationToken.None);
        //         if (isUsed)
        //         {
        //             throw new ApiException(localizer["Organization already has departments"],
        //                 statusCode: StatusCodes.Status409Conflict);
        //         }
        //
        //         await repository.DeleteByIdAsync(id);
        //         return TypedResults.NoContent();
        //     })
        //     .WithApiDescription(Tag, "DeleteOrganizationById", "Delete Organization by Id")
        //     .RequireAuthorization(AdminOnlyPolicy.Name)
        //     .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        
        _departmentsSubModule.MapEndpoints(endpoints);
        
        return endpoints;
    }
}