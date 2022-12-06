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
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Organizations.Dtos;
using Promasy.Modules.Organizations.Interfaces;
using Promasy.Modules.Organizations.Models;

namespace Promasy.Modules.Organizations;

internal class DepartmentsSubModule : SubModule
{
    private readonly SubDepartmentsSubModule _subDepartmentsSubModule;
    public override string Tag { get; } = "Department";
    public DepartmentsSubModule(string parentRoutePrefix) : base(parentRoutePrefix, "/departments")
    {
        _subDepartmentsSubModule = new SubDepartmentsSubModule($"{RoutePrefix}/{{departmentId:int}}");
    }

    public override IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        _subDepartmentsSubModule.RegisterServices(builder, configuration);
        return builder;
    }

    public override IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(RoutePrefix, async ([AsParameters] PagedRequest request, [FromRoute] int organizationId, [FromServices] IDepartmentsRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<PagedRequest>()
            .WithApiDescription(Tag, "GetDepartmentsList", "Get Departments list")
            .RequireAuthorization();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromRoute] int organizationId, [FromServices] IDepartmentsRepository repository) =>
            {
                var item = await repository.GetByIdAsync(id);
                if (item is null)
                {
                    throw new ApiException(null, StatusCodes.Status404NotFound);
                }
                return  TypedResults.Ok(item);
            })
            .WithApiDescription(Tag, "GetDepartmentById", "Get Department by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(RoutePrefix, async ([FromBody] CreateDepartmentRequest request, [FromRoute] int organizationId, [FromServices] IDepartmentsRepository repository,
                [FromServices] IStringLocalizer<SharedResource> localizer) =>
            {
                if (organizationId != request.OrganizationId)
                {
                    throw new ApiException(localizer["Incorrect organization id"]);
                }

                var id = await repository.CreateAsync(new DepartmentDto(0, request.Name, request.OrganizationId));
                var unit = await repository.GetByIdAsync(id);

                return TypedResults.Json(unit, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateDepartmentRequest>()
            .WithApiDescription(Tag, "CreateDepartment", "Create Department")
            .RequireAuthorization();

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateDepartmentRequest request, [FromRoute] int id, [FromRoute] int organizationId, [FromServices] IDepartmentsRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (organizationId != request.OrganizationId)
                    {
                        throw new ApiException(localizer["Incorrect organization id"]);
                    }
                    
                    if (request.Id != id)
                    {
                        throw new ApiException(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new DepartmentDto(request.Id, request.Name, request.OrganizationId));

                    return TypedResults.Accepted(string.Empty);
                })
            .WithValidator<UpdateDepartmentRequest>()
            .WithApiDescription(Tag, "UpdateDepartment", "Update Department")
            .RequireAuthorization();

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromRoute] int organizationId, [FromServices] IDepartmentsRepository repository,
                [FromServices] IDepartmentRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
            {
                var isEditable = rules.IsEditable(id);
                if (!isEditable)
                {
                    throw new ApiException(localizer["You cannot perform this action"],
                        statusCode: StatusCodes.Status409Conflict);
                }
                
                var isUsed = await rules.IsUsedAsync(id, CancellationToken.None);
                if (isUsed)
                {
                    throw new ApiException(localizer["Department has subdepartments"],
                        statusCode: StatusCodes.Status409Conflict);
                }

                await repository.DeleteByIdAsync(id);
                return TypedResults.NoContent();
            })
            .WithApiDescription(Tag, "DeleteDepartmentById", "Delete Department by Id")
            .RequireAuthorization()
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        
        _subDepartmentsSubModule.MapEndpoints(endpoints);
        
        return endpoints;
    }
}