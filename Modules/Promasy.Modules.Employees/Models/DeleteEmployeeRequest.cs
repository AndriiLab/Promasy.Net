using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Employees.Interfaces;

namespace Promasy.Modules.Employees.Models;

public record DeleteEmployeeRequest(int Id) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class DeleteEmployeeRequestValidator : AbstractPermissionsValidator<DeleteEmployeeRequest>
{
    public DeleteEmployeeRequestValidator(IEmployeeRules rules, IUserContext userContext, IStringLocalizer<SharedResource> localizer)
        : base(rules, userContext, localizer)
    {
    }
}
