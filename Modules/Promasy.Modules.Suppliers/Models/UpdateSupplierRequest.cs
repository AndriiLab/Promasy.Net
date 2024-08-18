using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Suppliers.Interfaces;

namespace Promasy.Modules.Suppliers.Models;

public record UpdateSupplierRequest(int Id, string Name, string? Comment, string? Phone) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class UpdateManufacturerRequestValidator : AbstractPermissionsValidator<UpdateSupplierRequest>
{
    public UpdateManufacturerRequestValidator(ISupplierRules rules, IStringLocalizer<SharedResource> localizer, IUserContext userContext) : base(rules, userContext, localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);
        
        RuleFor(r => r.Comment)
            .MaximumLength(PersistenceConstant.FieldLarge);
        
        RuleFor(r => r.Phone)
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.Id)
            .MustAsync(rules.IsExistsAsync)
            .WithMessage(localizer["Item not exist"]);
        
        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, t))
            .WithName(nameof(UpdateSupplierRequest.Name))
            .WithMessage(localizer["Name must be unique"]);
    }
}