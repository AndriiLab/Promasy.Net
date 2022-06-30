using Microsoft.AspNetCore.Http;

namespace Promasy.Modules.Core.Responses;

public static class PromasyResults
{
    public static IResult ValidationError(string error, int statusCode = StatusCodes.Status400BadRequest)
    {
        return Results.Json(new ValidationErrorResponse(error), statusCode: statusCode);
    }
}