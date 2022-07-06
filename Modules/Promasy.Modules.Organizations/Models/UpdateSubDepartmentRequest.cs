using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Models;

public record UpdateSubDepartmentRequest(int Id, string Name, int DepartmentId);

internal class UpdateSubDepartmentRequestValidator : AbstractValidator<UpdateSubDepartmentRequest>
{
    public UpdateSubDepartmentRequestValidator(ISubDepartmentsRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r)
            .Must(r => rules.IsEditable(r.Id))
            .WithMessage(localizer["You cannot perform this action"])
            .MustAsync((r, t) => rules.IsExistAsync(r.Id, r.DepartmentId, t))
            .WithMessage(localizer["Item not exist"])
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, r.DepartmentId, t))
            .WithMessage(localizer["Name must be unique"]);
    }
}