using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Promasy.Modules.Core.Validation;

public static class FluentValidationExtensions
{
    public static RouteHandlerBuilder WithValidator<TModel>(this RouteHandlerBuilder builder)
        where TModel : class
    {
        builder.AddEndpointFilter(async (context, next) =>
            {
                var validator = context.HttpContext.RequestServices.GetService<IValidator<TModel>>();
                ArgumentNullException.ThrowIfNull(validator, nameof(validator));

                var model = context.GetArgument<TModel>(0);
                ArgumentNullException.ThrowIfNull(model, nameof(model));

                var result = await validator.ValidateAsync(model);
                if (result.IsValid)
                {
                    return await next(context);
                }

                return TypedResults.ValidationProblem(result.ToDictionary());
            })
            .Produces<HttpValidationProblemDetails>(StatusCodes.Status400BadRequest);

        return builder;
    }
}