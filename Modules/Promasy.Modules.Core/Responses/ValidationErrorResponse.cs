namespace Promasy.Modules.Core.Responses;

public record ValidationErrorResponse(IDictionary<string, string[]> Errors)
{
    public ValidationErrorResponse(string error) 
        : this(new Dictionary<string, string[]> {{string.Empty, new[] {error}}})
    {
    }
}