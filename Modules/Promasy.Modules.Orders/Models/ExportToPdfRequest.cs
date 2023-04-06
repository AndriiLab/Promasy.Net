using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Domain.Employees;
using Promasy.Modules.Orders.Interfaces;

namespace Promasy.Modules.Orders.Models;

public record ExportToPdfRequest(int[] OrderIds, Dictionary<RoleName, int> SignEmployees);

internal class ExportToPdfRequestValidator : AbstractValidator<ExportToPdfRequest>
{
    public ExportToPdfRequestValidator(IOrderRules orderRules, IEmployeeRules employeeRules, IStringLocalizer<SharedResource> localizer, IStringLocalizer<RoleName> roleLocalizer)
    {
        RuleForEach(r => r.OrderIds)
            .MustAsync(orderRules.IsExistsAsync)
            .WithMessage((_, id) => string.Format(localizer["Order {0} not exist"], id));
        
        RuleForEach(r => r.SignEmployees)
            .MustAsync((kv, ct) => employeeRules.IsExistsWithRoleAsync(kv.Value, kv.Key, ct))
            .WithMessage((_, kv) => string.Format(localizer["Employee {0} not exist or not have Role {1}"], kv.Value, roleLocalizer[kv.Key.ToString()]));
    }
}