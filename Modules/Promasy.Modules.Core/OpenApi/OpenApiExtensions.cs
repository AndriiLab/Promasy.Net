using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Promasy.Modules.Core.OpenApi;

public static class OpenApiExtensions
{
    public static RouteHandlerBuilder WithApiDescription(this RouteHandlerBuilder builder, string tag, string id,
        string summary, string? description = null)
    {
        builder.WithName(id)
            .WithSummary(summary ?? id)
            .WithTags(tag);

        if (description is not null)
        {
            builder.WithDescription(description);
        }

        return builder;
    }
}