using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Units.Interfaces;

namespace Promasy.Modules.Units.Models;

public record DeleteUnitRequest(int Id) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class DeleteUnitRequestValidator  : AbstractPermissionsValidator<DeleteUnitRequest>
{
    public DeleteUnitRequestValidator(IUnitRules rules, IUserContext userContext, IStringLocalizer<SharedResource> localizer)
        : base(rules, userContext, localizer)
    {
        RuleFor(x => x.Id)
            .MustAsync(async (i, ct) => await rules.IsUsedAsync(i, ct) == false)
            .WithMessage(localizer["Unit already associated with order"])
            .WithErrorCode(StatusCodes.Status409Conflict.ToString()); // TODO: implement support of error codes
    }
}