namespace Promasy.Modules.Core.Dtos;

public abstract record EntityDto(int Id, int EditorId, string Editor, DateTime EditedDate);