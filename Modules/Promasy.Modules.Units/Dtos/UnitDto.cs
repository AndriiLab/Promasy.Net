using Promasy.Domain.Orders;
using Promasy.Modules.Core.Dtos;
using Riok.Mapperly.Abstractions;

namespace Promasy.Modules.Units.Dtos;

internal record UnitDto(int Id, string Name, int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);

internal record CreateUnitDto(string Name);

internal record UpdateUnitDto(int Id, string Name);


[Mapper]
internal static partial class UpdateUnitDtoMapper
{
    public static partial Unit MapFromSource(CreateUnitDto src);
    
    [MapperIgnoreSource(nameof(UpdateUnitDto.Id))]
    public static partial void CopyFromSource(UpdateUnitDto src, Unit tgt);
}
