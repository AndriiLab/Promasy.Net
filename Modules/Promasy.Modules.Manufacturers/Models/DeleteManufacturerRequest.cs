using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Manufacturers.Interfaces;

namespace Promasy.Modules.Manufacturers.Models;

public record DeleteManufacturerRequest(int Id) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class DeleteManufacturerRequestValidator : AbstractPermissionsValidator<DeleteManufacturerRequest>
{
    public DeleteManufacturerRequestValidator(IManufacturerRules rules, IUserContext userContext,
        IStringLocalizer<SharedResource> localizer) : base(rules, userContext, localizer)
    {
        RuleFor(r => r.Id)
            .MustAsync(async (i, c) => await rules.IsUsedAsync(i, c) == false)
            .WithMessage(localizer["Manufacturer already associated with order"])
            .WithErrorCode(StatusCodes.Status409Conflict.ToString());
    }
}