using Riok.Mapperly.Abstractions;

namespace Promasy.Modules.Cpv.Dtos;

internal record CpvDto(int Id, string Code, string DescriptionEnglish, string DescriptionUkrainian, int Level,
    bool IsTerminal, int? ParentId);
    
[Mapper]
internal static partial class CpvMapper
{
    public static partial IQueryable<CpvDto> MapFromQueryableSource(IQueryable<Domain.Vocabulary.Cpv> src);
}