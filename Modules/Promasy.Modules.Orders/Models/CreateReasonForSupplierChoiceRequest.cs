using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Orders.Interfaces;

namespace Promasy.Modules.Orders.Models;

public record CreateReasonForSupplierChoiceRequest(string Name);

internal class CreateReasonForSupplierChoiceRequestValidator : AbstractValidator<CreateReasonForSupplierChoiceRequest>
{
    public CreateReasonForSupplierChoiceRequestValidator(IReasonForSupplierChoiceRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium)
            .MustAsync(rules.IsNameUniqueAsync)
            .WithMessage(localizer["Name must be unique"]);
    }
}