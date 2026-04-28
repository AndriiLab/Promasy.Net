using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Units.Dtos;
using Promasy.Modules.Units.Interfaces;
using Riok.Mapperly.Abstractions;

namespace Promasy.Modules.Units.Models;

public record CreateUnitRequest(string Name)  : IRequestWithPermissionValidation
{
    public int GetId() => throw new NotSupportedException();
}

[Mapper]
internal static partial class CreateUnitRequestMapper
{
    public static partial CreateUnitDto MapFromSource(CreateUnitRequest src);
}

internal class CreateUnitRequestValidator : AbstractPermissionsValidator<CreateUnitRequest>
{
    public CreateUnitRequestValidator(IUnitRules rules, IUserContext userContext, IStringLocalizer<SharedResource> localizer) : base(rules, userContext, localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium)
            .MustAsync(rules.IsNameUniqueAsync)
            .WithMessage(localizer["Name must be unique"]);
    }
}