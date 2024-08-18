using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Models;

public record UpdateSubDepartmentRequest(int Id, string Name, int DepartmentId) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class UpdateSubDepartmentRequestValidator : AbstractPermissionsValidator<UpdateSubDepartmentRequest>
{
    public UpdateSubDepartmentRequestValidator(ISubDepartmentRules rules, IStringLocalizer<SharedResource> localizer, IUserContext userContext)
        : base(rules, userContext, localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsExistsAsync(r.Id, t))
            .WithMessage(localizer["Item not exist"]);
            
        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, r.DepartmentId, t))
            .WithMessage(localizer["Name must be unique"])
            .WithName(nameof(UpdateSubDepartmentRequest.Name));
    }
}