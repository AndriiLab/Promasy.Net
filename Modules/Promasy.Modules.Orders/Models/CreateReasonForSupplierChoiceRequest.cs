using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Orders.Interfaces;

namespace Promasy.Modules.Orders.Models;

public record CreateReasonForSupplierChoiceRequest(string Name) : IRequestWithPermissionValidation
{
    public int GetId() => throw new NotSupportedException();
}

internal class CreateReasonForSupplierChoiceRequestValidator : AbstractPermissionsValidator<CreateReasonForSupplierChoiceRequest>
{
    public CreateReasonForSupplierChoiceRequestValidator(IReasonForSupplierChoiceRules rules, IUserContext userContext, IStringLocalizer<SharedResource> localizer) : base(rules, userContext, localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium)
            .MustAsync(rules.IsNameUniqueAsync)
            .WithMessage(localizer["Name must be unique"]);
    }
}