using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Core.UserContext;
using Promasy.Domain.Employees;
using Promasy.Modules.Core.Auth;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Core.Validation;
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
        endpoints.MapGet($"{RoutePrefix}/roles", ([FromServices] IStringLocalizer<RoleName> localizer) =>
            {
                return Results.Json(Enum.GetValues<RoleName>()
                    .Select(r => new SelectItem<int>((int) r, localizer[r.ToString()])));
            })
            .WithTags(Tag)
            .WithName("Get available roles")
            .Produces<SelectItem<int>[]>();
                
        endpoints.MapPost($"{RoutePrefix}/{{id:int}}/change-password",
                async ([FromBody] PasswordChangeRequest request, [FromRoute] int id, IUserContext userContext, IAuthService authService, 
                    IStringLocalizer<SharedResource> localizer) =>
                {
                    if (!userContext.HasRoles((int)RoleName.Administrator) && userContext.GetId() != id)
                    {
                        return PromasyResults.ValidationError(localizer["Unable to modify other user password"]);
                    }

                    await authService.ChangePasswordAsync(id, request.Password);

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