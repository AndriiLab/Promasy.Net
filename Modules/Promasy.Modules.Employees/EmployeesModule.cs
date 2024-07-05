using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Domain.Employees;
using Promasy.Modules.Core.Exceptions;
using Promasy.Modules.Core.Mapper;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.OpenApi;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Employees.Dtos;
using Promasy.Modules.Employees.Interfaces;
using Promasy.Modules.Employees.Models;

namespace Promasy.Modules.Employees;

public class EmployeesModule : IModule
{
    public string Tag { get; } = "Employee";
    public string RoutePrefix { get; } = "/api/employees";

    public IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        return builder;
    }

    public WebApplication MapEndpoints(WebApplication app)
    {
        app.MapGet(RoutePrefix, async ([AsParameters] EmployeesPagedRequest request, [FromServices] IEmployeesRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<EmployeesPagedRequest>()
            .WithApiDescription(Tag, "GetEmployeesList", "Get Employees list")
            .RequireAuthorization();
        
        app.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromServices] IEmployeesRepository repository) =>
            {
                var employee = await repository.GetByIdAsync(id);
                if (employee is null)
                {
                    throw new ApiException(null, StatusCodes.Status404NotFound);
                }
                return TypedResults.Ok(employee) ;
            })
            .WithApiDescription(Tag, "GetEmployeeById", "Get Employee by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status404NotFound);
        
        app.MapPost(RoutePrefix, async ([FromBody] CreateEmployeeRequest request, [FromServices] IEmployeesRepository repository, [FromServices] IAuthService authService,  [FromServices] IMapper<CreateEmployeeRequest, CreateEmployeeDto> mapper) =>
            {
                var id = await repository.CreateAsync(mapper.MapFromSource(request));

                await authService.SetEmployeePasswordAsync(id, request.Password);
                
                var employee = await repository.GetByIdAsync(id);

                return TypedResults.Json(employee, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateEmployeeRequest>()
            .WithAuthorization(app, Tag, "Create Employee", PermissionTag.Create, RoleName.Administrator);

        app.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateEmployeeRequest request, [FromRoute] int id, [FromServices] IEmployeesRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer, [FromServices] IMapper<UpdateEmployeeRequest, UpdateEmployeeDto> mapper) =>
                {
                    if (request.Id != id)
                    {
                        throw new ApiException(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(mapper.MapFromSource(request));

                    return TypedResults.Accepted($"{RoutePrefix}/{request.Id}");
                })
            .WithAuthorizationAndValidation<UpdateEmployeeRequest>(app, Tag, "Update Employee", PermissionTag.Update,
                Enum.GetValues<RoleName>().Select(r =>
                (r, r switch
                    {
                        RoleName.Administrator => PermissionCondition.None,
                        _ => PermissionCondition.SameUser
                    })).ToArray());


        app.MapDelete($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromServices] IEmployeesRepository repository,
                [FromServices] IEmployeeRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
            {
                var isEditable = rules.IsEditable(id);
                if (!isEditable)
                {
                    throw new ApiException(localizer["You cannot perform this action"],
                        statusCode: StatusCodes.Status409Conflict);
                }

                await repository.DeleteByIdAsync(id);
                return TypedResults.NoContent();
            })
            .WithAuthorization(app, Tag, "Delete Employee by Id", PermissionTag.Delete, RoleName.Administrator)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
                
        app.MapPost($"{RoutePrefix}/{{id:int}}/change-password",
                async ([AsParameters] PasswordChangeRequest request, IEmployeeRules rules, IAuthService authService, 
                    IStringLocalizer<SharedResource> localizer) =>
                {
                    if (!rules.CanChangePasswordForEmployee(request.Id))
                    {
                        throw new ApiException(localizer["Unable to modify other user password"]);
                    }

                    await authService.SetEmployeePasswordAsync(request.Id, request.Password);

                    return TypedResults.Accepted($"{RoutePrefix}/{request.Id}");
                })
            .WithAuthorizationAndValidation<PasswordChangeRequest>(app, Tag, "Change Employee password", s => PermissionTag.Update($"{s}/Password"),
                Enum.GetValues<RoleName>().Select(r =>
                (r, r switch
                {
                    RoleName.Administrator => PermissionCondition.None,
                    _ => PermissionCondition.SameUser
                })).ToArray());
        
        return app;
    }
}