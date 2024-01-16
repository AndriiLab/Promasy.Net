using Promasy.Modules.Core.Mapper;
using Riok.Mapperly.Abstractions;

namespace Promasy.Modules.Cpv.Dtos;

internal record CpvDto(int Id, string Code, string DescriptionEnglish, string DescriptionUkrainian, int Level,
    bool IsTerminal, int? ParentId);
    
[Mapper]
internal partial class CpvMapper : IQueryableMapper<Domain.Vocabulary.Cpv, CpvDto>
{
    public partial CpvDto MapFromSource(Domain.Vocabulary.Cpv src);
    public partial IQueryable<CpvDto> MapFromQueryableSource(IQueryable<Domain.Vocabulary.Cpv> src);
}