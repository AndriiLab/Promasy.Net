using Promasy.Application.Interfaces;
using Promasy.Domain.Finances;
using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Finances.Interfaces;

internal interface IFinanceSubDepartmentRules : IRules<FinanceSubDepartment>
{
    Task<bool> IsUniqueFinanceSubDepartmentAsync(int financeSourceId, int subDepartmentId, CancellationToken ct);
    Task<bool> IsUniqueFinanceSubDepartmentAsync(int id, int financeSourceId, int subDepartmentId, CancellationToken ct);
    Task<bool> CanBeAssignedAsEquipmentAsync(decimal amount, int financeSourceId, CancellationToken ct);
    Task<bool> CanBeAssignedAsEquipmentAsync(decimal amount, int id, int financeSourceId, CancellationToken ct);
    Task<bool> CanBeAssignedAsMaterialsAsync(decimal amount, int financeSourceId, CancellationToken ct);
    Task<bool> CanBeAssignedAsMaterialsAsync(decimal amount, int id, int financeSourceId, CancellationToken ct);
    Task<bool> CanBeAssignedAsServicesAsync(decimal amount, int financeSourceId, CancellationToken ct);
    Task<bool> CanBeAssignedAsServicesAsync(decimal amount, int id,  int financeSourceId, CancellationToken ct);
}