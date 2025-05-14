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

    public override WebApplication MapEndpoints(WebApplication app)
    {
        app.MapGet(RoutePrefix, async ([AsParameters] PagedRequest request, [FromRoute] int organizationId, [FromServices] IDepartmentsRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<PagedRequest>()
            .WithApiDescription(Tag, "GetDepartmentsList", "Get Departments list")
            .RequireAuthorization();

        app.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromRoute] int organizationId, [FromServices] IDepartmentsRepository repository) =>
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

        app.MapPost(RoutePrefix, async ([FromBody] CreateDepartmentRequest request, [FromRoute] int organizationId, [FromServices] IDepartmentsRepository repository,
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
            .WithAuthorizationAndValidation<CreateDepartmentRequest>(app, Tag, "Create Department", PermissionAction.Create,
                (RoleName.Administrator, PermissionCondition.Allowed), 
                (RoleName.Director, PermissionCondition.SameOrganization),
                (RoleName.DeputyDirector, PermissionCondition.SameOrganization));

        app.MapPut($"{RoutePrefix}/{{id:int}}",
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
            .WithAuthorizationAndValidation<UpdateDepartmentRequest>(app, Tag, "Update Department", PermissionAction.Update, 
                (RoleName.Administrator, PermissionCondition.Allowed),
                (RoleName.Director, PermissionCondition.SameOrganization),
                (RoleName.DeputyDirector, PermissionCondition.SameOrganization),
                (RoleName.HeadOfDepartment, PermissionCondition.SameDepartment),
                (RoleName.PersonallyLiableEmployee, PermissionCondition.SameDepartment));

        app.MapDelete($"{RoutePrefix}/{{id:int}}", async ([AsParameters] DeleteDepartmentRequest request, [FromServices] IDepartmentsRepository repository) =>
            {
                await repository.DeleteByIdAsync(request.Id);
                return TypedResults.NoContent();
            })
            .WithAuthorizationAndValidation<DeleteDepartmentRequest>(app, Tag, "Delete Department by Id", PermissionAction.Delete,
                (RoleName.Administrator, PermissionCondition.Allowed), 
                (RoleName.Director, PermissionCondition.SameOrganization),
                (RoleName.DeputyDirector, PermissionCondition.SameOrganization))
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
        
        _subDepartmentsSubModule.MapEndpoints(app);
        
        return app;
    }
}