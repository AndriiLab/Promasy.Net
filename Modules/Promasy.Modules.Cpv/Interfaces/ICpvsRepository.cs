using Promasy.Modules.Core.Modules;
using Promasy.Modules.Cpv.Dtos;

namespace Promasy.Modules.Cpv.Interfaces;

internal interface ICpvsRepository : IRepository
{
    Task<List<CpvDto>> GetCpvsAsync(int? level, int? parentId, string? term);
    Task<CpvDto?> GetCpvByCodeAsync(string code);
}