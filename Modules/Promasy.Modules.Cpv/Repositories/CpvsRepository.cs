using System.Text;
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

        if (parentId.HasValue)
        {
            query = query.Where(c => c.ParentId == parentId);
        }

        if (!string.IsNullOrEmpty(search))
        {
            var isCode = search.Length <= 10 && search.Any(c => char.IsDigit(c) || c == '-');
            if (isCode)
            {
                var code = TrimCode(search);
                var codeLevel = code.Length > 8 ? 8 : code.Length - 1;
                query = query.Where(c => c.Code.StartsWith(code));
                query = query.Where(c => c.Level == codeLevel);
            }
            else
            {
                query = query.Where(c => EF.Functions.ToTsVector("simple", c.DescriptionUkrainian)
                                             .Matches(EF.Functions.PlainToTsQuery("simple", search)) ||
                                         EF.Functions.ToTsVector("english", c.DescriptionEnglish)
                                             .Matches(EF.Functions.PlainToTsQuery("english", search)));
            }
            
        } else if (level.HasValue)
        {
            query = query.Where(c => c.Level == level);
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
    
    private static string TrimCode(string search)
    {
        var sb = new StringBuilder(20);
        foreach (var ci in search.Select((c, i) => new { Char = c, Index = i }))
        {
            if (ci.Index == 0 || char.IsDigit(ci.Char) && ci.Char > 48)
            {
                sb.Append(ci.Char);
                continue;
            }
            break;
        }

        return sb.ToString();
    }
}