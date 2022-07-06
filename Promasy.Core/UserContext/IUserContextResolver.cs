namespace Promasy.Core.UserContext;

public interface IUserContextResolver
{
    IUserContext? Resolve();
    void Set(IUserContext context);
}