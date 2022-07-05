using Promasy.Modules.Core.Dtos;

namespace Promasy.Modules.Organizations.Dtos;

public record DepartmentDto(int Id, string Name, int OrganizationId, int EditorId = default, string Editor = "", DateTime EditedDate = default)
    : EntityDto(Id, EditorId, Editor, EditedDate);