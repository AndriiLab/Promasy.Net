﻿using System.Text;
using Microsoft.EntityFrameworkCore;
using Promasy.Application.Interfaces;
using Promasy.Modules.Core.Mapper;
using Promasy.Modules.Cpv.Dtos;
using Promasy.Modules.Cpv.Interfaces;
using Promasy.Modules.Cpv.Models;

namespace Promasy.Modules.Cpv.Repositories;

internal class CpvsRepository : ICpvsRepository
{
    private readonly IDatabase _database;
    private readonly IQueryableMapper<Domain.Vocabulary.Cpv, CpvDto> _mapper;

    public CpvsRepository(IDatabase database, IQueryableMapper<Domain.Vocabulary.Cpv, CpvDto> mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    public async Task<List<CpvDto>> GetCpvsAsync(GetCpvsRequest request)
    {
        var query = _database.Cpvs
            .AsNoTracking();

        if (request.Id.HasValue)
        {
            query = query.Where(c => c.Id == request.Id);
        } 
        else if (request.ParentId.HasValue)
        {
            query = query.Where(c => c.ParentId == request.ParentId);
        } 
        else if (!string.IsNullOrEmpty(request.Search))
        {
            var isCode = request.Search.Length <= 10 && request.Search.Any(c => char.IsDigit(c) || c == '-');
            if (isCode)
            {
                var code = TrimCode(request.Search);
                var codeLevel = code.Length > 8 ? 8 : code.Length - 1;
                query = query.Where(c => c.Code.StartsWith(code));
                query = query.Where(c => c.Level == codeLevel);
            }
            else
            {
                query = query.Where(c => EF.Functions.ToTsVector("simple", c.DescriptionUkrainian)
                                             .Matches(EF.Functions.PlainToTsQuery("simple", request.Search)) ||
                                         EF.Functions.ToTsVector("english", c.DescriptionEnglish)
                                             .Matches(EF.Functions.PlainToTsQuery("english", request.Search)));
            }
        }
        else
        {
            query = query.Where(c => c.Level == 1);
        }

        var list = await _mapper.MapFromQueryableSource(query).ToListAsync();

        return list;
    }

    public Task<CpvDto?> GetCpvByCodeAsync(string code)
    {
        return _mapper.MapFromQueryableSource(_database.Cpvs
            .AsNoTracking()
            .Where(c => c.Code == code))
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