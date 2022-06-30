using Microsoft.AspNetCore.Http;

namespace Promasy.Modules.Core.Extensions;

public static class HttpContextExtensions
{
    private const string ForwarderFor = "X-Forwarded-For";

    public static string? GetIpAddress(this HttpContext ctx)
    {
        return ctx.Request.Headers.TryGetValue(ForwarderFor, out var ip) 
            ? ip
            : ctx.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    }
}