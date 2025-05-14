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

    public override WebApplication MapEndpoints(WebApplication app)
    {
        app.MapGet(RoutePrefix, async ([AsParameters] PagedRequest request, [FromRoute] int organizationId, [FromRoute] int departmentId, [FromServices] ISubDepartmentsRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(departmentId, request);
                return TypedResults.Ok(list);
            })
            .WithValidator<PagedRequest>()
            .WithApiDescription(Tag, "GetSubDepartmentsList", "Get SubDepartments list")
            .RequireAuthorization();

        app.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromRoute] int organizationId, 
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

        app.MapPost(RoutePrefix, async ([FromBody]CreateSubDepartmentRequest request, [FromRoute] int organizationId,
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
            .WithAuthorizationAndValidation<CreateSubDepartmentRequest>(app, Tag, "Create SubDepartment", PermissionAction.Create,
                (RoleName.Administrator, PermissionCondition.Allowed), 
                (RoleName.Director, PermissionCondition.SameOrganization),
                (RoleName.DeputyDirector, PermissionCondition.SameOrganization),
                (RoleName.HeadOfDepartment, PermissionCondition.SameDepartment),
                (RoleName.PersonallyLiableEmployee, PermissionCondition.SameDepartment));


        app.MapPut($"{RoutePrefix}/{{id:int}}",
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
            .WithAuthorizationAndValidation<UpdateSubDepartmentRequest>(app, Tag, "Update SubDepartment", PermissionAction.Update,
                (RoleName.Administrator, PermissionCondition.Allowed), 
                (RoleName.Director, PermissionCondition.SameOrganization),
                (RoleName.DeputyDirector, PermissionCondition.SameOrganization),
                (RoleName.HeadOfDepartment, PermissionCondition.SameDepartment),
                (RoleName.PersonallyLiableEmployee, PermissionCondition.SameDepartment));

        app.MapDelete($"{RoutePrefix}/{{id:int}}", async ([AsParameters] DeleteSubDepartmentRequest request, [FromServices] ISubDepartmentsRepository repository) =>
            {
                await repository.DeleteByIdAsync(request.Id);
                return TypedResults.NoContent();
            })
            .WithAuthorizationAndValidation<DeleteSubDepartmentRequest>(app, Tag, "Delete SubDepartment by Id", PermissionAction.Delete,
                (RoleName.Administrator, PermissionCondition.Allowed), 
                (RoleName.Director, PermissionCondition.SameOrganization),
                (RoleName.DeputyDirector, PermissionCondition.SameOrganization),
                (RoleName.HeadOfDepartment, PermissionCondition.SameDepartment),
                (RoleName.PersonallyLiableEmployee, PermissionCondition.SameDepartment))
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        
        return app;
    }
}