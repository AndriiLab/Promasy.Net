using Promasy.Modules.Core.Modules;
using Promasy.Modules.Cpv.Dtos;
using Promasy.Modules.Cpv.Models;

namespace Promasy.Modules.Cpv.Interfaces;

internal interface ICpvsRepository : IRepository
{
    Task<List<CpvDto>> GetCpvsAsync(GetCpvsRequest request);
    Task<CpvDto?> GetCpvByCodeAsync(string code);
}