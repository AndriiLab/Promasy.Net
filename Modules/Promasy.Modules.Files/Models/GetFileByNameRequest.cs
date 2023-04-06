using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;

namespace Promasy.Modules.Files.Models;

public record GetFileByNameRequest(string FileName);

internal partial class GetFilesByNameRequestValidator : AbstractValidator<GetFileByNameRequest>
{
    public GetFilesByNameRequestValidator(IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.FileName)
            .NotNull()
            .NotEmpty()
            .Must(fn => FileNameRegex().IsMatch(fn))
            .WithMessage(localizer["File name incorrect"]);
    }

    [GeneratedRegex(@"^[\w-]+\.[A-Za-z]{3}$")]
    private static partial Regex FileNameRegex();
}