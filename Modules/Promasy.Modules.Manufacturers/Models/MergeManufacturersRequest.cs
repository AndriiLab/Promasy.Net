using FluentValidation;
using Promasy.Modules.Manufacturers.Interfaces;

namespace Promasy.Modules.Manufacturers.Models;

public record MergeManufacturersRequest(int TargetId, int[] SourceIds);

internal class MergeManufacturersRequestValidator : AbstractValidator<MergeManufacturersRequest>
{
    public MergeManufacturersRequestValidator(IManufacturersRules rules)
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
            .WithMessage("Manufacturer not exist");
    }
}