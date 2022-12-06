using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;

namespace Promasy.Modules.Cpv.Models;

public record GetCpvByCodeRequest(string Code);

internal class GetCpvByCodeRequestValidator : AbstractValidator<GetCpvByCodeRequest>
{
    public GetCpvByCodeRequestValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Code)
            .NotNull()
            .NotEmpty()
            .Length(10)
            .WithMessage(localizer["CPV code must be in format 12345678-9"]);
    }
}