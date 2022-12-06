using Microsoft.AspNetCore.Http.HttpResults;
using Promasy.Core.Exceptions;
using Promasy.Modules.Core.Exceptions;

namespace Promasy.Web.App.ExceptionHandlers;

public class PromasyExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public PromasyExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next.Invoke(ctx);
        }
        catch (ApiException apiEx)
        {
            if (string.IsNullOrEmpty(apiEx.Message))
            {
                ctx.Response.StatusCode = apiEx.StatusCode;
            }
            else
            {
                await WriteResultAsync(ctx, TypedResults.Problem(apiEx.Message, statusCode: apiEx.StatusCode));
            }
        }
        catch (PromasyException pEx)
        {
            await WriteResultAsync(ctx, TypedResults.Problem(pEx.Message));
        }
    }

    private static async Task WriteResultAsync(HttpContext ctx, ProblemHttpResult result)
    {
        ctx.Response.ContentType = result.ContentType;
        ctx.Response.StatusCode = result.StatusCode;
        await ctx.Response.WriteAsJsonAsync(result.ProblemDetails);
    }
}