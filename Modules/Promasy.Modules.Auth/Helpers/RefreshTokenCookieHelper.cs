using Microsoft.AspNetCore.Http;

namespace Promasy.Modules.Auth.Helpers;

internal static class RefreshTokenCookieHelper
{
    private const string RefreshTokenCookieKey = "refresh_token";
    
    public static void SetCookie(HttpResponse response, string token, DateTime refreshExpiryTime)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = refreshExpiryTime
        };
        response.Cookies.Append(RefreshTokenCookieKey, token, cookieOptions);
    }

    public static string? GetFromCookie(HttpRequest request)
    {
        return request.Cookies.TryGetValue(RefreshTokenCookieKey, out var rt) ? rt : null;
    }
}