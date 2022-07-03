using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Units.Dtos;

internal record UnitDto(int Id, string Name, int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);
