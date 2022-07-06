using Promasy.Modules.Core.Modules;

namespace Promasy.Modules.Core.Auth;

public interface IAuthService : IService
{
    Task<int?> AuthAsync(string userName, string password);
    Task SetUserContextAsync(int id);
    Task ChangePasswordAsync(int userId, string newPassword);
}