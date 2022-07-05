using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Promasy.Core;
using Promasy.Core.Resources;

namespace Promasy.Modules.Cpv.Models;

public class GetCpvByCodeRequest
{
    public string Code { get; set; }

    public GetCpvByCodeRequest()
    {
        Code = string.Empty;
    }

    public GetCpvByCodeRequest(string code)
    {
        Code = code;
    }
    
    public static ValueTask<GetCpvByCodeRequest?> BindAsync(HttpContext httpContext)
    {
        var result = new GetCpvByCodeRequest(
            httpContext.Request.RouteValues.TryGetValue("code", out var code) ? code as string ?? string.Empty : string.Empty);

        return ValueTask.FromResult<GetCpvByCodeRequest?>(result);
    }
}

internal class GetCpvByCodeRequestValidator : AbstractValidator<GetCpvByCodeRequest>
{
    public GetCpvByCodeRequestValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Code)
            .NotEmpty()
            .Length(10)
            .WithMessage(localizer["CPV code must be in format 12345678-9"]);
    }
}