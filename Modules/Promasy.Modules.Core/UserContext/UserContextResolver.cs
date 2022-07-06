using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Promasy.Core.Extensions;
using Promasy.Core.UserContext;
using Promasy.Modules.Core.Extensions;

namespace Promasy.Modules.Core.UserContext;

public class UserContextResolver : IUserContextResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserContextResolver> _logger;
    private IUserContext? _userContext;

    public UserContextResolver(IHttpContextAccessor httpContextAccessor, ILogger<UserContextResolver> logger)
    {
        _userContext = null;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public IUserContext? Resolve()
    {
        if(_userContext is not null)
        {
            return _userContext;
        }

        var context = _httpContextAccessor.HttpContext;
        var principal = context?.User;
        if (principal is null)
        {
            _logger.LogInformation("User context not initialized");
            _userContext = null;
            return null;
        }
        var userId = principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.Id).ToCamelCase())?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogInformation("User not authorized. User context not initialized");
            _userContext = null;
            return null;
        }

        var roles = principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value
            .Split(",")
            .Select(r => Convert.ToInt32(r))
            .ToArray();
        
        var user = new UserContext(
            Convert.ToInt32(userId),
            principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.FirstName).ToCamelCase())?.Value ?? string.Empty, 
            principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.MiddleName).ToCamelCase())?.Value ?? string.Empty, 
            principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.LastName).ToCamelCase())?.Value ?? string.Empty, 
            principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value ?? string.Empty,
            principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.Organization).ToCamelCase())?.Value ?? string.Empty,
            Convert.ToInt32(principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.OrganizationId).ToCamelCase())?.Value),
            principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.Department).ToCamelCase())?.Value ?? string.Empty,
            Convert.ToInt32(principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.DepartmentId).ToCamelCase())?.Value),
            principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.SubDepartment).ToCamelCase())?.Value ?? string.Empty,
            Convert.ToInt32(principal.Claims.FirstOrDefault(x => x.Type == nameof(UserContext.SubDepartmentId).ToCamelCase())?.Value),
            context!.GetIpAddress(),
            roles ?? Array.Empty<int>());
        
        _userContext = user;
        _logger.LogInformation("User context initialized for {Email}", _userContext.Email);
        return user;
    }

    public void Set(IUserContext context)
    {
        _userContext = context;
        _logger.LogInformation("User context set for {Email}", _userContext.Email);
    }
}