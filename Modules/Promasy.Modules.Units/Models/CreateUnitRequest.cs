using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
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

internal class CreateUnitRequestValidator : AbstractValidator<CreateUnitRequest>
{
    public CreateUnitRequestValidator(IUnitRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium)
            .MustAsync(rules.IsNameUniqueAsync)
            .WithMessage(localizer["Name must be unique"]);
    }
}