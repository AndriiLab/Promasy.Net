using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Mapper;
using Promasy.Modules.Units.Dtos;
using Promasy.Modules.Units.Interfaces;
using Riok.Mapperly.Abstractions;

namespace Promasy.Modules.Units.Models;

public record CreateUnitRequest(string Name);

[Mapper]
internal partial class CreateUnitRequestMapper : IMapper<CreateUnitRequest, CreateUnitDto>
{
    public partial CreateUnitDto MapFromSource(CreateUnitRequest src);
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