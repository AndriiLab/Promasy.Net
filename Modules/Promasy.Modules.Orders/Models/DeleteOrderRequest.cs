using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Orders.Interfaces;

namespace Promasy.Modules.Orders.Models;

public record DeleteOrderRequest(int Id) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class DeleteOrderRequestValidator : AbstractPermissionsValidator<DeleteOrderRequest>
{
    public DeleteOrderRequestValidator(IOrderRules rules, IUserContext userContext, IStringLocalizer<SharedResource> localizer)
        : base(rules, userContext, localizer)
    {
    }
}