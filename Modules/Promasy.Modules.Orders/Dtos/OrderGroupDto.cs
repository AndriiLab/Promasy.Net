using Promasy.Domain.Employees;
using Promasy.Domain.Orders;
using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Orders.Dtos;

public record OrderGroupDto(int Id, string FileKey, OrderGroupStatus Status,
        ICollection<int> OrderIds, ICollection<OrderGroupEmployeeDto> Employees,
        int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);

public record OrderGroupEmployeeDto(int EmployeeId, RoleName Role);