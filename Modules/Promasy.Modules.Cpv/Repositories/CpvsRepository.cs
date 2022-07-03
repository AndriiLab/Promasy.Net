using Microsoft.EntityFrameworkCore;
using Promasy.Domain.Persistence;
using Promasy.Modules.Cpv.Dtos;
using Promasy.Modules.Cpv.Interfaces;

namespace Promasy.Modules.Cpv.Repositories;

internal class CpvsRepository : ICpvsRepository
{
    private readonly IDatabase _database;

    public CpvsRepository(IDatabase database)
    {
        _database = database;
    }

    public async Task<List<CpvDto>> GetCpvsAsync(int? level, int? parentId, string? search)
    {
        var query = _database.Cpvs
            .AsNoTracking();

        if (level.HasValue)
        {
            query = query.Where(c => c.Level == level);
        }

        if (parentId.HasValue)
        {
            query = query.Where(c => c.ParentId == parentId);
        }

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c => c.Code.StartsWith(search) || 
                                     c.DescriptionUkrainian.Contains(search) || 
                                     c.DescriptionEnglish.Contains(search));
        }

        var list = await query.Select(c => new CpvDto(c.Id, c.Code, c.DescriptionEnglish, c.DescriptionUkrainian,
                c.Level, c.IsTerminal, c.ParentId))
            .ToListAsync();

        return list;
    }

    public Task<CpvDto?> GetCpvByCodeAsync(string code)
    {
        return _database.Cpvs
            .AsNoTracking()
            .Where(c => c.Code == code)
            .Select(c => new CpvDto(c.Id, c.Code, c.DescriptionEnglish, c.DescriptionUkrainian, c.Level, c.IsTerminal,
                c.ParentId))
            .FirstOrDefaultAsync();
    }
}