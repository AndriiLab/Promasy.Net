using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Core.Rules;

public interface IFinanceSourceRules : IRules
{
    Task<bool> IsNumberUniqueAsync(string number, int year, CancellationToken ct);
    Task<bool> IsNumberUniqueAsync(string number, int year, int id, CancellationToken ct);
}