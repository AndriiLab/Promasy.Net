using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;

namespace Promasy.Modules.Core.OpenApi;

public static class OpenApiExtensions
{
    public static RouteHandlerBuilder WithApiDescription(this RouteHandlerBuilder builder, string tag, string id,
        string? summary = null, string? description = null)
    {
        builder.WithOpenApi(o => new OpenApiOperation(o)
        {
            OperationId = id,
            Summary = summary ?? id,
            Description = description,
            Tags = new List<OpenApiTag> { new() { Name = tag } }
        });

        return builder;
    }
}