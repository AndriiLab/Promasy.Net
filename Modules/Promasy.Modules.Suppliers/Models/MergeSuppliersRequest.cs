using FluentValidation;
using Promasy.Modules.Suppliers.Interfaces;

namespace Promasy.Modules.Suppliers.Models;

public record MergeSuppliersRequest(int TargetId, int[] SourceIds);

internal class MergeManufacturersRequestValidator : AbstractValidator<MergeSuppliersRequest>
{
    public MergeManufacturersRequestValidator(ISuppliersRules rules)
    {
        RuleFor(_ => _.SourceIds)
            .NotEmpty();

        RuleFor(_ => _)
            .Must((r) => !r.SourceIds.Contains(r.TargetId))
            .WithMessage("Target must not be included in source list")
            .Must(r => rules.IsMergeable())
            .WithMessage("You cannot perform this action");

        RuleFor(r => r.TargetId)
            .MustAsync(rules.IsExistAsync)
            .WithMessage("Supplier not exist");
    }
}