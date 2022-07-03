using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Manufacturers.Dtos;

internal record ManufacturerDto(int Id, string Name, int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);