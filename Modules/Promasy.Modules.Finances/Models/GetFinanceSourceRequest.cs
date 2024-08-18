using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Finances.Interfaces;

namespace Promasy.Modules.Finances.Models;

public record GetFinanceSourceRequest(int Id) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class GetFinanceSourceRequestValidator : AbstractPermissionsValidator<GetFinanceSourceRequest>
{
    public GetFinanceSourceRequestValidator(IFinanceSourceRules rules, IUserContext userContext, IStringLocalizer<SharedResource> localizer)
        : base(rules, userContext, localizer)
    {
    }
}