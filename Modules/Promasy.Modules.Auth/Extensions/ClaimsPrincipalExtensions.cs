using System.Security.Claims;

namespace Promasy.Modules.Auth.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetClaimOrDefault(this ClaimsPrincipal? principal, string claimName, string defaultValue = "")
    {
        return principal?.Claims.FirstOrDefault(c => c.Type == claimName)?.Value ?? defaultValue;
    }
}