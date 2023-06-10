using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Exceptions;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.OpenApi;
using Promasy.Modules.Core.Policies;
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

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(RoutePrefix, async ([AsParameters] EmployeesPagedRequest request, [FromServices] IEmployeesRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<EmployeesPagedRequest>()
            .WithApiDescription(Tag, "GetEmployeesList", "Get Employees list")
            .RequireAuthorization();
        
        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromServices] IEmployeesRepository repository) =>
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
        
        endpoints.MapPost(RoutePrefix, async ([FromBody] CreateEmployeeRequest request, [FromServices] IEmployeesRepository repository, [FromServices] IAuthService authService) =>
            {
                var id = await repository.CreateAsync(new CreateEmployeeDto(request.FirstName, request.MiddleName, request.LastName, request.Email,
                    request.PrimaryPhone, request.ReservePhone, request.UserName, request.SubDepartmentId, request.Roles));

                await authService.SetEmployeePasswordAsync(id, request.Password);
                
                var employee = await repository.GetByIdAsync(id);

                return TypedResults.Json(employee, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateEmployeeRequest>()
            .WithApiDescription(Tag, "CreateEmployee", "Create Employee")
            .RequireAuthorization(AdminOnlyPolicy.Name);

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateEmployeeRequest request, [FromRoute] int id, [FromServices] IEmployeesRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.Id != id)
                    {
                        throw new ApiException(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new UpdateEmployeeDto(request.Id, request.FirstName, request.MiddleName,
                        request.LastName, request.Email, request.PrimaryPhone, request.ReservePhone, request.SubDepartmentId,
                        request.Roles));

                    return TypedResults.Accepted($"{RoutePrefix}/{request.Id}");
                })
            .WithValidator<UpdateEmployeeRequest>()
            .WithApiDescription(Tag, "UpdateEmployee", "Update Employee")
            .RequireAuthorization();

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromServices] IEmployeesRepository repository,
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
            .WithApiDescription(Tag, "DeleteEmployee", "Delete Employee by Id")
            .RequireAuthorization(AdminOnlyPolicy.Name)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);
                
        endpoints.MapPost($"{RoutePrefix}/{{id:int}}/change-password",
                async ([FromBody] PasswordChangeRequest request, [FromRoute] int id, IEmployeeRules rules, IAuthService authService, 
                    IStringLocalizer<SharedResource> localizer) =>
                {
                    if (!rules.CanChangePasswordForEmployee(id))
                    {
                        throw new ApiException(localizer["Unable to modify other user password"]);
                    }

                    await authService.SetEmployeePasswordAsync(id, request.Password);

                    return TypedResults.Accepted($"{RoutePrefix}/{id}");
                })
            .WithValidator<PasswordChangeRequest>()
            .WithApiDescription(Tag, "ChangeEmployeePassword", "Change Employee password")
            .RequireAuthorization();
        
        return endpoints;
    }
}