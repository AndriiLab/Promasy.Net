using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Mapper;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Units.Dtos;
using Promasy.Modules.Units.Interfaces;
using Riok.Mapperly.Abstractions;

namespace Promasy.Modules.Units.Models;

public record UpdateUnitRequest(int Id, string Name) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
};

[Mapper]
internal partial class UpdateUnitRequestMapper : IMapper<UpdateUnitRequest, UpdateUnitDto>
{
    public partial UpdateUnitDto MapFromSource(UpdateUnitRequest src);
}

internal class UpdateUnitRequestValidator : AbstractPermissionsValidator<UpdateUnitRequest>
{
    public UpdateUnitRequestValidator(IUnitRules rules, IUserContext userContext, IStringLocalizer<SharedResource> localizer)
        : base(rules, userContext,localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.Id)
            .MustAsync(rules.IsExistsAsync)
            .WithMessage(localizer["Item not exist"]);
        
        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, t))
            .WithName(nameof(UpdateUnitRequest.Name))
            .WithMessage(localizer["Name must be unique"]);
    }
}