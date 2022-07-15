using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Domain.Employees;
using Promasy.Modules.Core.Auth;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Policies;
using Promasy.Modules.Core.Responses;
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
        endpoints.MapGet($"{RoutePrefix}/all-roles", ([FromServices] IStringLocalizer<RoleName> localizer) =>
            {
                return Results.Json(Enum.GetValues<RoleName>()
                    .Select(r => new SelectItem<int>((int) r, localizer[r.ToString()]))
                    .OrderBy(r => r.Value));
            })
            .WithTags(Tag)
            .WithName("Get available roles")
            .Produces<SelectItem<int>[]>();
        
        endpoints.MapGet(RoutePrefix, async (EmployeesPagedRequest request, [FromServices] IEmployeesRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return Results.Json(list);
            })
            .WithValidator<EmployeesPagedRequest>()
            .WithTags(Tag)
            .WithName("Get Employees list")
            .RequireAuthorization()
            .Produces<PagedResponse<EmployeeShortDto>>();
        
        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IEmployeesRepository repository) =>
            {
                var unit = await repository.GetByIdAsync(id);
                return unit is not null ? Results.Json(unit) : Results.NotFound();
            })
            .WithTags(Tag)
            .WithName("Get Employee by Id")
            .RequireAuthorization()
            .Produces<EmployeeDto>()
            .Produces(StatusCodes.Status404NotFound);
        
        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateEmployeeRequest request, [FromServices] IEmployeesRepository repository, [FromServices] IAuthService authService) =>
            {
                var id = await repository.CreateAsync(new CreateEmployeeDto(request.FirstName, request.MiddleName, request.LastName, request.Email,
                    request.PrimaryPhone, request.ReservePhone, request.UserName, request.SubDepartmentId, request.Roles));

                await authService.SetEmployeePasswordAsync(id, request.Password);
                
                var unit = await repository.GetByIdAsync(id);

                return Results.Json(unit, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateEmployeeRequest>()
            .WithTags(Tag)
            .WithName("Create Employee")
            .RequireAuthorization(AdminOnlyPolicy.Name)
            .Produces<EmployeeDto>(StatusCodes.Status201Created);

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateEmployeeRequest request, [FromRoute] int id, [FromServices] IEmployeesRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.Id != id)
                    {
                        return PromasyResults.ValidationError(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new UpdateEmployeeDto(request.Id, request.FirstName, request.MiddleName,
                        request.LastName, request.Email, request.PrimaryPhone, request.ReservePhone, request.SubDepartmentId,
                        request.Roles));

                    return Results.Ok(StatusCodes.Status202Accepted);
                })
            .WithValidator<UpdateEmployeeRequest>()
            .WithTags(Tag)
            .WithName("Update Employee")
            .RequireAuthorization()
            .Produces<EmployeeDto>(StatusCodes.Status202Accepted);

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IEmployeesRepository repository,
                [FromServices] IEmployeeRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
            {
                var isEditable = rules.IsEditable(id);
                if (!isEditable)
                {
                    return PromasyResults.ValidationError(localizer["You cannot perform this action"],
                        StatusCodes.Status409Conflict);
                }

                await repository.DeleteByIdAsync(id);
                return Results.Ok(StatusCodes.Status204NoContent);
            })
            .WithTags(Tag)
            .WithName("Delete Employee by Id")
            .RequireAuthorization(AdminOnlyPolicy.Name)
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationErrorResponse>(StatusCodes.Status409Conflict);
                
        endpoints.MapPost($"{RoutePrefix}/{{id:int}}/change-password",
                async ([FromBody] PasswordChangeRequest request, [FromRoute] int id, IEmployeeRules rules, IAuthService authService, 
                    IStringLocalizer<SharedResource> localizer) =>
                {
                    if (!rules.CanChangePasswordForEmployee(id))
                    {
                        return PromasyResults.ValidationError(localizer["Unable to modify other user password"]);
                    }

                    await authService.SetEmployeePasswordAsync(id, request.Password);

                    return Results.Ok(StatusCodes.Status202Accepted);
                })
            .WithValidator<PasswordChangeRequest>()
            .WithTags(Tag)
            .WithName("Change password")
            .RequireAuthorization()
            .Produces(StatusCodes.Status202Accepted);
        
        return endpoints;
    }
}