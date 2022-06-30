using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Promasy.Core.Extensions;
using Promasy.Modules.Core.Responses;

namespace Promasy.Modules.Core.Validation;

/// <summary>
/// Minimal API Validator using package FluentValidation
/// </summary>
/// Based on solution from <see ref="https://github.com/juniorporfirio/MinimalApis.Validators/blob/main/src/MininalApis.Validators/FluentValidationExtension.cs"/>
public static class FluentValidationExtensions
{
    public static RouteHandlerBuilder WithValidator<TModel>(this RouteHandlerBuilder builder)
        where TModel : class
    {
        builder.Add(endpoint =>
        {
            var original = endpoint.RequestDelegate;
            endpoint.RequestDelegate = async ctx =>
            {
                var validator = ctx.RequestServices.GetService<IValidator<TModel>>();

                ArgumentNullException.ThrowIfNull(validator, nameof(validator));

                var bindingMethod = typeof(TModel).GetMethod("BindAsync", BindingFlags.Public | BindingFlags.Static);
                TModel? model;
                var isBodyRead = false;
                if (bindingMethod is not null)
                {
                    model = await bindingMethod.InvokeAsync(null, ctx) as TModel;
                } 
                else
                {
                    ctx.Request.EnableBuffering();
                    model = await ctx.Request.ReadFromJsonAsync<TModel>();
                    isBodyRead = true;
                }

                ArgumentNullException.ThrowIfNull(model, nameof(model));

                var result = await validator.ValidateAsync(model);

                if (!result.IsValid)
                {
                    ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                    ctx.Response.ContentType = "application/problem+json";
                    await ctx.Response.WriteAsJsonAsync(new ValidationErrorResponse(result.ToDictionary()));
                    return;
                }

                if (isBodyRead)
                {
                    ctx.Request.Body.Position = 0;
                }

                await original!(ctx);
            };
        });

        builder.Produces<ValidationErrorResponse>(StatusCodes.Status400BadRequest);

        return builder;
    }

    private static IDictionary<string, string[]> ToDictionary(this ValidationResult validationResult)
        => validationResult.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray()
            );
}