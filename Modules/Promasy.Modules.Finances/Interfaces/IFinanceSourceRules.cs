using Promasy.Application.Interfaces;
using Promasy.Domain.Finances;
using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Finances.Interfaces;

internal interface IFinanceSourceRules : IRules<FinanceSource>
{
    Task<bool> IsNumberUniqueAsync(string number, int year, CancellationToken ct);
    Task<bool> IsNumberUniqueAsync(string number, int year, int id, CancellationToken ct);
}