using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Promasy.Core.UserContext;
using Promasy.Modules.Auth.Extensions;
using Promasy.Modules.Core.Extensions;

namespace Promasy.Modules.Auth.UserContext;

public class UserContext : IUserContext
{
    private readonly HttpContext? _httpContext;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext;
    }

    public bool IsAuthenticated() => GetPrincipal()?.Identity?.IsAuthenticated ?? false;
    public int GetId() => Convert.ToInt32(GetPrincipal()?.Identity?.Name);
    public string GetFirstName() => GetPrincipal().GetClaimOrDefault(ClaimTypes.GivenName);
    public string GetLastName() => GetPrincipal().GetClaimOrDefault(ClaimTypes.Surname);
    public string GetEmail() => GetPrincipal().GetClaimOrDefault(ClaimTypes.Email);
    public string GetOrganization() => GetPrincipal().GetClaimOrDefault(PromasyClaims.Organization);
    public int GetOrganizationId() => Convert.ToInt32(GetPrincipal().GetClaimOrDefault(PromasyClaims.OrganizationId, "0"));
    public string GetDepartment() => GetPrincipal().GetClaimOrDefault(PromasyClaims.Department);
    public int GetDepartmentId() => Convert.ToInt32(GetPrincipal().GetClaimOrDefault(PromasyClaims.DepartmentId));
    public string GetSubDepartment() => GetPrincipal().GetClaimOrDefault(PromasyClaims.SubDepartment);
    public int GetSubDepartmentId()  => Convert.ToInt32(GetPrincipal().GetClaimOrDefault(PromasyClaims.SubDepartmentId));
    public string? GetIpAddress() => _httpContext?.GetIpAddress();
    public bool HasRoles(params int[] roles) => roles.Any(r => GetPrincipal()?.IsInRole(r.ToString()) ?? false);
    
    private ClaimsPrincipal? GetPrincipal() => _httpContext?.User;
}