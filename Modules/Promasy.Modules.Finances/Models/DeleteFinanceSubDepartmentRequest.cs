using Promasy.Modules.Core.Permissions;

namespace Promasy.Modules.Finances.Models;

public record DeleteFinanceSubDepartmentRequest(int FinanceId, int SubDepartmentId) : IRequestWithPermissionValidation
{
    public int GetId() => throw new NotSupportedException();
}