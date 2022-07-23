using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Domain.Finances;
using Promasy.Modules.Finances.Interfaces;

namespace Promasy.Modules.Finances.Models;

public record CreateFinanceSourceRequest(string Number, string Name, FinanceFundType FundType,
    DateOnly Start, DateOnly End, string Kpkvk,
    decimal TotalEquipment, decimal TotalMaterials, decimal TotalServices);

internal class CreateFinanceSourceRequestValidator : AbstractValidator<CreateFinanceSourceRequest>
{
    public CreateFinanceSourceRequestValidator(IFinanceSourceRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Number)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMini);
        
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);
        
        RuleFor(r => r.Kpkvk)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(r => r.TotalEquipment)
            .GreaterThanOrEqualTo(0);
        
        RuleFor(r => r.TotalMaterials)
            .GreaterThanOrEqualTo(0);
        
        RuleFor(r => r.TotalServices)
            .GreaterThanOrEqualTo(0);

        RuleFor(r => r)
            .Must(r => r.Start <= r.End)
            .WithName(nameof(CreateFinanceSourceRequest.Start))
            .WithMessage(localizer["Start date cannot be ahead End date"]);
        
        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNumberUniqueAsync(r.Name, r.Start.Year, t))
            .WithName(nameof(CreateFinanceSourceRequest.Number))
            .WithMessage(localizer["Number already defined in this year"]);
    }
}