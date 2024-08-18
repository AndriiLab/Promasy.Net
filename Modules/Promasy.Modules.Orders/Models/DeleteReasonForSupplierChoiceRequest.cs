using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Orders.Interfaces;

namespace Promasy.Modules.Orders.Models;

public record DeleteReasonForSupplierChoiceRequest(int Id) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class DeleteReasonForSupplierChoiceRequestValidator : AbstractPermissionsValidator<DeleteReasonForSupplierChoiceRequest>
{
    public DeleteReasonForSupplierChoiceRequestValidator(IReasonForSupplierChoiceRules rules, IUserContext userContext, IStringLocalizer<SharedResource> localizer)
        : base(rules, userContext, localizer)
    {
        RuleFor(r => r.Id)
            .MustAsync(async (i, c) => await rules.IsUsedAsync(i, c) == false)
            .WithMessage(localizer["Reason for Supplier Choice already associated with order"])
            .WithErrorCode(StatusCodes.Status409Conflict.ToString());

    }
}