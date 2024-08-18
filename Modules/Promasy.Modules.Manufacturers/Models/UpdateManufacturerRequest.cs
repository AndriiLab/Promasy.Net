using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Manufacturers.Interfaces;

namespace Promasy.Modules.Manufacturers.Models;

public record UpdateManufacturerRequest(int Id, string Name) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class UpdateManufacturerRequestValidator : AbstractPermissionsValidator<UpdateManufacturerRequest>
{
    public UpdateManufacturerRequestValidator(IManufacturerRules rules, IUserContext userContext, IStringLocalizer<SharedResource> localizer) : base(rules, userContext, localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.Id)
            .MustAsync(rules.IsExistsAsync)
            .WithMessage(localizer["Manufacturer not exist"]);
        
        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, t))
            .WithMessage(localizer["Name must be unique"])
            .WithName(nameof(UpdateManufacturerRequest.Name));
    }
}