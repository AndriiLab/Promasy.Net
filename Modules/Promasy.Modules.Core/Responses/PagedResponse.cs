namespace Promasy.Modules.Core.Responses;

public class PagedResponse<TElement>
{
    public int Page { get; set; }
    public int Total { get; set; }
    public ICollection<TElement> Collection { get; set; } = new List<TElement>();
}