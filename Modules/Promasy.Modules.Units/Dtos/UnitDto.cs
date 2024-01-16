using Promasy.Domain.Orders;
using Promasy.Modules.Core.Dtos;
using Promasy.Modules.Core.Mapper;
using Riok.Mapperly.Abstractions;

namespace Promasy.Modules.Units.Dtos;

internal record UnitDto(int Id, string Name, int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);

internal record CreateUnitDto(string Name);

internal record UpdateUnitDto(int Id, string Name);


[Mapper]
internal partial class UpdateUnitDtoMapper : ISyncMapper<CreateUnitDto, UpdateUnitDto, Unit>
{
    public partial Unit MapFromSource(CreateUnitDto src);
    
    [MapperIgnoreTarget(nameof(Unit.Id))]
    public partial void CopyFromSource(UpdateUnitDto src, Unit tgt);
}
