using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Suppliers.Interfaces;

namespace Promasy.Modules.Suppliers.Models;

public record DeleteSupplierRequest(int Id) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class DeleteSupplierRequestValidator : AbstractPermissionsValidator<DeleteSupplierRequest>
{
    public DeleteSupplierRequestValidator(ISupplierRules rules, IUserContext userContext, IStringLocalizer<SharedResource> localizer)
        : base(rules, userContext, localizer)
    {
        
        RuleFor(r => r.Id)
            .MustAsync(async (i, c) => await rules.IsUsedAsync(i, c) == false)
            .WithMessage(localizer["Supplier already associated with order"])
            .WithErrorCode(StatusCodes.Status409Conflict.ToString());
    }
}