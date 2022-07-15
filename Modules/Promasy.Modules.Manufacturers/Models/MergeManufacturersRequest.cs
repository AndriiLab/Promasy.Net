using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Modules.Manufacturers.Interfaces;

namespace Promasy.Modules.Manufacturers.Models;

public record MergeManufacturersRequest(int TargetId, int[] SourceIds);

internal class MergeManufacturersRequestValidator : AbstractValidator<MergeManufacturersRequest>
{
    public MergeManufacturersRequestValidator(IManufacturerRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(_ => _.SourceIds)
            .NotEmpty();

        RuleFor(_ => _)
            .Must((r) => !r.SourceIds.Contains(r.TargetId))
            .WithMessage(localizer["Target must not be included in source list"])
            .WithName(nameof(MergeManufacturersRequest.TargetId));

        RuleFor(r => r.TargetId)
            .MustAsync(rules.IsExistsAsync)
            .WithMessage(localizer["Item not exist"]);
    }
}