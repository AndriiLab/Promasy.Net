using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Promasy.Core.UserContext;
using Promasy.Domain.Employees;
using Promasy.Domain.Persistence;
using Promasy.Modules.Auth.Interfaces;
using Z.EntityFramework.Plus;

namespace Promasy.Modules.Auth.Repositories;

internal class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDatabase _database;
    private readonly IUserContextResolver _userContextResolver;
    private readonly IUserContext _userContext;
    private readonly ILogger<RefreshTokenRepository> _logger;

    public RefreshTokenRepository(IDatabase database, IUserContextResolver userContextResolver, ILogger<RefreshTokenRepository> logger)
    {
        _database = database;
        _userContext = userContextResolver.Resolve()!;
        _userContextResolver = userContextResolver;
        _logger = logger;
    }

    public async Task CreateAsync(string token, int userId, DateTime expires)
    {
        var userContext = _userContextResolver.Resolve();
        if (userContext is null)
        {
            throw new NoNullAllowedException("User context must be initialized");
        }

        await CleanupOldTokensAsync();

        await _database.RefreshTokens
            .Where(r => r.EmployeeId == userId)
            .Where(r => r.Revoked == null)
            .Where(r => r.Expires > DateTime.UtcNow)
            .UpdateAsync(r => new RefreshToken
            {
                Revoked = DateTime.UtcNow,
                ModifierId = userId,
                ModifiedDate = DateTime.UtcNow,
                ReasonRevoked = TokenRevokeReason.Revoked,
                RevokedByIp = userContext.IpAddress
            });
        
        var rt = new RefreshToken
        {
            Token = token,
            Expires = expires,
            CreatedByIp = userContext.IpAddress ?? string.Empty,
            EmployeeId = userId
        };
        _database.RefreshTokens.Add(rt);
        await _database.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(string oldToken, string newToken, DateTime newExpires)
    {
        await CleanupOldTokensAsync();

        var oldRt = await _database.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == oldToken);
        if (oldRt is null || oldRt.IsExpired())
        {
            return false;
        }
        
        var userContext = _userContextResolver.Resolve();
        if (userContext is null)
        {
            throw new NoNullAllowedException("User context must be initialized");
        }
        
        if (oldRt.IsRevoked())
        {
            _logger.LogWarning("Found compromised Refresh Token {Id} for user {UserId}", oldRt.Id, oldRt.EmployeeId);
            var childTokens = await _database.RefreshTokens
                .FromSqlInterpolated(@$"
                WITH RECURSIVE ""Initial"" AS (
                SELECT *
                    FROM ""RefreshTokens""
                WHERE ""ReplacedByTokenId"" = {oldRt.Id}
                UNION ALL
                SELECT S.*
                    FROM ""Initial"" I
                    JOIN ""RefreshTokens"" S ON S.""Id"" = I.""ReplacedByTokenId""
                    )
                SELECT *
                    FROM ""Initial""
                ORDER BY ""Id""")
                .ToListAsync();

            foreach (var t in childTokens.Where(ct => ct.IsActive()).Concat(new []{ oldRt }))
            {
                t.Revoked = DateTime.UtcNow;
                t.ReasonRevoked = TokenRevokeReason.Compromised;
                t.RevokedByIp = userContext.IpAddress;
            }
                
            await _database.SaveChangesAsync();
            
            return false;
        }
        
        var rt = new RefreshToken
        {
            Token = newToken,
            Expires = newExpires,
            CreatedByIp = userContext.IpAddress ?? string.Empty,
            EmployeeId = oldRt.EmployeeId
        };
        _database.RefreshTokens.Add(rt);
        oldRt.ReplacedByTokenId = rt.Id;
        oldRt.Revoked = DateTime.UtcNow;
        oldRt.ReasonRevoked = TokenRevokeReason.Replaced;
        oldRt.RevokedByIp = userContext.IpAddress;
        await _database.SaveChangesAsync();

        return true;
    }

    public async Task RevokeAsync(string token)
    {
        await CleanupOldTokensAsync();

        var rt = await _database.RefreshTokens
            .Where(t => t.Token == token)
            .FirstOrDefaultAsync();
        if (rt is null || !rt.IsActive())
        {
            return;
        }
        
        rt.Revoked = DateTime.UtcNow;
        rt.ReasonRevoked = TokenRevokeReason.Revoked;
        rt.RevokedByIp = _userContext.IpAddress;
        await _database.SaveChangesAsync();
    }

    // todo: rewrite to scheduled task
    private Task CleanupOldTokensAsync()
    {
        return _database.RefreshTokens
            .Where(rt => rt.Expires < DateTime.UtcNow.AddDays(-14))
            .DeleteAsync();
    }
}