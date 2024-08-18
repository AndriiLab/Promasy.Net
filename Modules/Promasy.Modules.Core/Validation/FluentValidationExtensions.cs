using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Promasy.Domain.Employees;
using Promasy.Modules.Core.Permissions;

namespace Promasy.Modules.Core.Validation;

public static class FluentValidationExtensions
{
    public static RouteHandlerBuilder WithValidator<TModel>(this RouteHandlerBuilder builder, int argumentIndex = 0)
        where TModel : class
    {
        builder.AddEndpointFilter(async (context, next) =>
            {
                var validator = context.HttpContext.RequestServices.GetService<IValidator<TModel>>();
                ArgumentNullException.ThrowIfNull(validator);

                var model = context.GetArgument<TModel>(argumentIndex);
                ArgumentNullException.ThrowIfNull(model);

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
    
    public static RouteHandlerBuilder WithPermissionsValidator<TModel>(this RouteHandlerBuilder builder,
        IReadOnlyCollection<(RoleName Role, PermissionCondition Condition)> roleConditions, int argumentIndex = 0)
        where TModel : IRequestWithPermissionValidation
    {
        builder.AddEndpointFilter(async (context, next) =>
            {
                var validator = context.HttpContext.RequestServices.GetService<IPermissionsValidator<TModel>>();
                ArgumentNullException.ThrowIfNull(validator);
                
                validator.SetRoleConditions(roleConditions);

                var model = context.GetArgument<TModel>(argumentIndex);
                ArgumentNullException.ThrowIfNull(model);

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