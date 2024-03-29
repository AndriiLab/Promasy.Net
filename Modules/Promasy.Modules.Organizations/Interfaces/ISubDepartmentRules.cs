﻿using Promasy.Application.Interfaces;
using Promasy.Domain.Organizations;
using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Organizations.Interfaces;

internal interface ISubDepartmentRules : IRules<SubDepartment>
{
    Task<bool> IsNameUniqueAsync(string name, int departmentId, CancellationToken ct);
    Task<bool> IsNameUniqueAsync(string name, int id, int departmentId, CancellationToken ct);
    bool IsEditable(int id);
    Task<bool> IsUsedAsync(int id, CancellationToken ct);
}