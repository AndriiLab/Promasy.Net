using Promasy.Modules.Core.Permissions;

namespace Promasy.Modules.Finances.Models;

public record DeleteFinanceSourceRequest(int Id) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}