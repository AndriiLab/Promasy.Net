using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Orders.Dtos;

internal record ReasonForSupplierChoiceDto(int Id, string Name, int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);
