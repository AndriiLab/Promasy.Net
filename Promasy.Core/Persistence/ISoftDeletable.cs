namespace Promasy.Core.Persistence;

public interface ISoftDeletable
{
    bool Deleted { get; set; }
}