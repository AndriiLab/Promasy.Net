using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Promasy.Core.UserContext;
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
                
        endpoints.MapPost($"{RoutePrefix}/{{id:int}}/change-password",
                async ([FromBody] PasswordChangeRequest request, [FromRoute] int id, IUserContext userContext, IAuthService authService) =>
                {
                    if (!userContext.IsAdmin() && userContext.Id != id)
                    {
                        return PromasyResults.ValidationError("Unable to modify other user password");
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