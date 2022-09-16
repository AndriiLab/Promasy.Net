using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Modules.Orders.Interfaces;

namespace Promasy.Modules.Orders.Models;

public record MergeReasonForSupplierChoiceRequest(int TargetId, int[] SourceIds);

internal class MergeReasonForSupplierChoiceRequestValidator : AbstractValidator<MergeReasonForSupplierChoiceRequest>
{
    public MergeReasonForSupplierChoiceRequestValidator(IReasonForSupplierChoiceRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(_ => _.SourceIds)
            .NotEmpty();

        RuleFor(_ => _)
            .Must((r) => !r.SourceIds.Contains(r.TargetId))
            .WithName(nameof(MergeReasonForSupplierChoiceRequest.TargetId))
            .WithMessage(localizer["Target must not be included in source list"]);

        RuleFor(r => r.TargetId)
            .MustAsync(rules.IsExistsAsync)
            .WithMessage(localizer["Item not exist"]);
    }
}