using Promasy.Core.Exceptions;

namespace Promasy.Modules.Core.Exceptions;

public class RepositoryException : PromasyException
{
    public RepositoryException(string message) : base(message)
    {
    }
}