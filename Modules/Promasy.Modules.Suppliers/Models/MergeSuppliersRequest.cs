using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core;
using Promasy.Core.Resources;
using Promasy.Modules.Suppliers.Interfaces;

namespace Promasy.Modules.Suppliers.Models;

public record MergeSuppliersRequest(int TargetId, int[] SourceIds);

internal class MergeManufacturersRequestValidator : AbstractValidator<MergeSuppliersRequest>
{
    public MergeManufacturersRequestValidator(ISuppliersRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(_ => _.SourceIds)
            .NotEmpty();

        RuleFor(_ => _)
            .Must((r) => !r.SourceIds.Contains(r.TargetId))
            .WithMessage(localizer["Target must not be included in source list"]);

        RuleFor(r => r.TargetId)
            .MustAsync(rules.IsExistAsync)
            .WithMessage(localizer["Item not exist"]);
    }
}