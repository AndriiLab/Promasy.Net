using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Orders.Interfaces;

namespace Promasy.Modules.Orders.Models;

public record UpdateReasonForSupplierChoiceRequest(int Id, string Name);

internal class UpdateReasonForSupplierChoiceRequestValidator : AbstractValidator<UpdateReasonForSupplierChoiceRequest>
{
    public UpdateReasonForSupplierChoiceRequestValidator(IReasonForSupplierChoiceRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.Id)
            .MustAsync(rules.IsExistsAsync)
            .WithMessage(localizer["Item not exist"])
            .MustAsync(rules.IsEditableAsync)
            .WithMessage(localizer["You cannot perform this action"]);
        
        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, t))
            .WithName(nameof(UpdateReasonForSupplierChoiceRequest.Name))
            .WithMessage(localizer["Name must be unique"]);
    }
}