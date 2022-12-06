using Microsoft.AspNetCore.Http;
using Promasy.Core.Exceptions;

namespace Promasy.Modules.Core.Exceptions;

public class ApiException : PromasyException
{
    public int StatusCode { get; }
    public ApiException(string? message, int? statusCode = null) : base(message ?? string.Empty)
    {
        StatusCode = statusCode ?? StatusCodes.Status422UnprocessableEntity;
    }
}