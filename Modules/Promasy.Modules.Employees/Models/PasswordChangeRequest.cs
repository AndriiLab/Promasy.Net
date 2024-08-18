using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Employees.Interfaces;

namespace Promasy.Modules.Employees.Models;

public record PasswordChangeRequest(string Password, [FromRoute] int Id) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class PasswordChangeRequestValidator : AbstractPermissionsValidator<PasswordChangeRequest>
{
    public PasswordChangeRequestValidator(IEmployeeRules rules, IUserContext userContext, IStringLocalizer<SharedResource> localizer)
        : base(rules, userContext, localizer)
    {
        RuleFor(m => m.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(PersistenceConstant.FieldMedium);
    }
}