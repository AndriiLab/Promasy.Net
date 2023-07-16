using Promasy.Application.Interfaces;

namespace Promasy.Modules.Auth.Interfaces;

internal interface IRefreshTokenRepository : IRepository
{
    Task CreateAsync(string token, int userId, DateTime expires);
    Task<bool> UpdateAsync(string oldToken, string newToken, DateTime newExpires);
    Task RevokeAsync(string token);
}