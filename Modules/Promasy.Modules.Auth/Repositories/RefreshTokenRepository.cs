using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Promasy.Application.Interfaces;
using Promasy.Domain.Employees;
using Promasy.Modules.Auth.Interfaces;

namespace Promasy.Modules.Auth.Repositories;

internal class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDatabase _database;
    private readonly IUserContext _userContext;
    private readonly ILogger<RefreshTokenRepository> _logger;

    public RefreshTokenRepository(IDatabase database, IUserContext userContext, ILogger<RefreshTokenRepository> logger)
    {
        _database = database;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task CreateAsync(string token, int userId, DateTime expires)
    {
        if (!_userContext.IsAuthenticated())
        {
            throw new NoNullAllowedException("User context must be initialized");
        }

        await CleanupOldTokensAsync();

        await _database.RefreshTokens
            .Where(r => r.EmployeeId == userId)
            .Where(r => r.Revoked == null)
            .Where(r => r.Expires > DateTime.UtcNow)
            .ExecuteUpdateAsync(s => s
                .SetProperty(e => e.Revoked, DateTime.UtcNow)
                .SetProperty(e => e.ModifierId, userId)
                .SetProperty(e => e.ModifiedDate, DateTime.UtcNow)
                .SetProperty(e => e.ReasonRevoked, TokenRevokeReason.Revoked)
                .SetProperty(e => e.RevokedByIp, _userContext.GetIpAddress())
            );
        
        var rt = new RefreshToken
        {
            Token = token,
            Expires = expires,
            CreatedByIp = _userContext.GetIpAddress() ?? string.Empty,
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
        
        if (!_userContext.IsAuthenticated())
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
                t.RevokedByIp = _userContext.GetIpAddress();
            }
                
            await _database.SaveChangesAsync();
            
            return false;
        }
        
        var rt = new RefreshToken
        {
            Token = newToken,
            Expires = newExpires,
            CreatedByIp = _userContext.GetIpAddress() ?? string.Empty,
            EmployeeId = oldRt.EmployeeId
        };
        _database.RefreshTokens.Add(rt);
        oldRt.ReplacedByTokenId = rt.Id;
        oldRt.Revoked = DateTime.UtcNow;
        oldRt.ReasonRevoked = TokenRevokeReason.Replaced;
        oldRt.RevokedByIp = _userContext.GetIpAddress();
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
        rt.RevokedByIp = _userContext.GetIpAddress();
        await _database.SaveChangesAsync();
    }

    // todo: rewrite to scheduled task
    private Task<int> CleanupOldTokensAsync()
    {
        return _database.RefreshTokens
            .Where(rt => rt.Expires < DateTime.UtcNow.AddDays(-14))
            .ExecuteDeleteAsync();
    }
}