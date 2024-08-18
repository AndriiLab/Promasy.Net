using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Orders.Interfaces;

namespace Promasy.Modules.Orders.Models;

public record UpdateReasonForSupplierChoiceRequest(int Id, string Name) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class UpdateReasonForSupplierChoiceRequestValidator : AbstractPermissionsValidator<UpdateReasonForSupplierChoiceRequest>
{
    public UpdateReasonForSupplierChoiceRequestValidator(IReasonForSupplierChoiceRules rules, IStringLocalizer<SharedResource> localizer, IUserContext userContext) 
        : base(rules, userContext, localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.Id)
            .MustAsync(rules.IsExistsAsync)
            .WithMessage(localizer["Item not exist"]);
        
        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, t))
            .WithName(nameof(UpdateReasonForSupplierChoiceRequest.Name))
            .WithMessage(localizer["Name must be unique"]);
    }
}