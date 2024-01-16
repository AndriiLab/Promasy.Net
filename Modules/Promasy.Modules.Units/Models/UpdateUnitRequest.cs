using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Mapper;
using Promasy.Modules.Units.Dtos;
using Promasy.Modules.Units.Interfaces;
using Riok.Mapperly.Abstractions;

namespace Promasy.Modules.Units.Models;

public record UpdateUnitRequest(int Id, string Name);

[Mapper]
internal partial class UpdateUnitRequestMapper : IMapper<UpdateUnitRequest, UpdateUnitDto>
{
    public partial UpdateUnitDto MapFromSource(UpdateUnitRequest src);
}

internal class UpdateUnitRequestValidator : AbstractValidator<UpdateUnitRequest>
{
    public UpdateUnitRequestValidator(IUnitRules rules, IStringLocalizer<SharedResource> localizer)
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
            .WithName(nameof(UpdateUnitRequest.Name))
            .WithMessage(localizer["Name must be unique"]);
    }
}