using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Promasy.Core.Extensions;
using Promasy.Core.UserContext;
using Promasy.Modules.Core.Extensions;

namespace Promasy.Modules.Core.UserContext;

public interface IUserContextResolver
{
    IUserContext? Resolve();
}

public class UserContextResolver : IUserContextResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserContextResolver> _logger;

    public UserContextResolver(IHttpContextAccessor httpContextAccessor, ILogger<UserContextResolver> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public IUserContext? Resolve()
    {
        var context = _httpContextAccessor.HttpContext;
        var principal = context?.User;
        if (principal is null)
        {
            _logger.LogInformation("User context not initialized");
            return null;
        }
        var user = new UserContext(
            Convert.ToInt32(principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.Id).ToCamelCase())?.Value),
            principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.FirstName).ToCamelCase())?.Value ?? string.Empty, 
            principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.MiddleName).ToCamelCase())?.Value ?? string.Empty, 
            principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.LastName).ToCamelCase())?.Value ?? string.Empty, 
            principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.Email).ToCamelCase())?.Value ?? string.Empty,
            principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.Organization).ToCamelCase())?.Value ?? string.Empty,
            Convert.ToInt32(principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.OrganizationId).ToCamelCase())?.Value),
            principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.Department).ToCamelCase())?.Value ?? string.Empty,
            Convert.ToInt32(principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.DepartmentId).ToCamelCase())?.Value),
            principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.SubDepartment).ToCamelCase())?.Value ?? string.Empty,
            Convert.ToInt32(principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.SubDepartmentId).ToCamelCase())?.Value),
            context!.GetIpAddress(),
            principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.Roles).ToCamelCase())?.Value?.Split(",") ?? ArraySegment<string>.Empty);
        
        _logger.LogInformation("User context initialized for {Email}", user.Email);
        return user;
    }
}