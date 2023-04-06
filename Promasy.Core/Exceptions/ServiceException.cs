namespace Promasy.Core.Exceptions;

public class ServiceException : PromasyException
{
    public ServiceException(string message) : base(message)
    {
    }
}