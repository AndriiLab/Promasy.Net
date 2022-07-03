using FluentValidation;
using Promasy.Modules.Units.Interfaces;

namespace Promasy.Modules.Units.Models;

public record MergeUnitsRequest(int TargetId, int[] SourceIds);

internal class MergeUnitsRequestValidator : AbstractValidator<MergeUnitsRequest>
{
    public MergeUnitsRequestValidator(IUnitsRules rules)
    {
        RuleFor(_ => _.SourceIds)
            .NotEmpty();

        RuleFor(_ => _)
            .Must((r) => !r.SourceIds.Contains(r.TargetId))
            .WithMessage("Target must not be included in source list");

        RuleFor(r => r.TargetId)
            .MustAsync(rules.IsExistAsync)
            .WithMessage("Unit not exist");
    }
}