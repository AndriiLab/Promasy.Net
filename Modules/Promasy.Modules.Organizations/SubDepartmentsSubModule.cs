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

internal class SubDepartmentsSubModule : SubModule
{
    public override string Tag { get; } = "Sub-department";
    
    public SubDepartmentsSubModule(string parentRoutePrefix) : base(parentRoutePrefix, "/sub-departments")
    {
    }

    public override IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        return builder;
    }

    public override IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(RoutePrefix, async ([AsParameters] PagedRequest request, [FromRoute] int organizationId, [FromRoute] int departmentId, [FromServices] ISubDepartmentsRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(departmentId, request);
                return TypedResults.Ok(list);
            })
            .WithValidator<PagedRequest>()
            .WithApiDescription(Tag, "GetSubDepartmentsList", "Get SubDepartments list")
            .RequireAuthorization();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromRoute] int organizationId, 
                [FromRoute] int departmentId, [FromServices] ISubDepartmentsRepository repository) =>
            {
                var item = await repository.GetByIdAsync(id);
                if (item is null)
                {
                    throw new ApiException(null, StatusCodes.Status404NotFound);
                }
                return TypedResults.Ok(item);
            })
            .WithApiDescription(Tag, "GetSubDepartmentById", "Get SubDepartment by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateSubDepartmentRequest request, [FromRoute] int organizationId,
                [FromRoute] int departmentId,  [FromServices] ISubDepartmentsRepository repository,
                [FromServices] IStringLocalizer<SharedResource> localizer) =>
            {
                if (organizationId != request.OrganizationId)
                {
                    throw new ApiException(localizer["Incorrect organization id"]);
                }
                
                if (departmentId != request.DepartmentId)
                {
                    throw new ApiException(localizer["Incorrect department id"]);
                }
                
                var id = await repository.CreateAsync(new SubDepartmentDto(0, request.Name, request.DepartmentId));
                var unit = await repository.GetByIdAsync(id);

                return TypedResults.Json(unit, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateSubDepartmentRequest>()
            .WithApiDescription(Tag, "CreateSubDepartment", "Create SubDepartment")
            .RequireAuthorization();

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateSubDepartmentRequest request, [FromRoute] int id, [FromRoute] int organizationId,
                    [FromRoute] int departmentId, [FromServices] ISubDepartmentsRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (departmentId != request.DepartmentId)
                    {
                        throw new ApiException(localizer["Incorrect department id"]);
                    }
                    
                    if (request.Id != id)
                    {
                        throw new ApiException(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new SubDepartmentDto(request.Id, request.Name, request.DepartmentId));

                    return TypedResults.Accepted(string.Empty);
                })
            .WithValidator<UpdateSubDepartmentRequest>()
            .WithApiDescription(Tag, "UpdateSubDepartment", "Update SubDepartment")
            .RequireAuthorization();

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromRoute] int organizationId,
                [FromRoute] int departmentId, [FromServices] ISubDepartmentsRepository repository,
                [FromServices] ISubDepartmentRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
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
                    throw new ApiException(localizer["Unit already associated with order"],
                        statusCode: StatusCodes.Status409Conflict);
                }

                await repository.DeleteByIdAsync(id);
                return TypedResults.NoContent();
            })
            .WithApiDescription(Tag, "DeleteSubDepartmentById", "Delete SubDepartment by Id")
            .RequireAuthorization()
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        
        return endpoints;
    }
}