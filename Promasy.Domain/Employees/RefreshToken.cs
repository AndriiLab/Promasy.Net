using System;
using Promasy.Core.Persistence;

namespace Promasy.Domain.Employees;

public class RefreshToken : IEntity
{
    public int Id { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public string CreatedByIp { get; set; }

    public DateTime? Revoked { get; set; }
    public string? RevokedByIp { get; set; }
    public TokenRevokeReason? ReasonRevoked { get; set; }
    
    public int? ReplacedByTokenId { get; set; }
    public int EmployeeId { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public int CreatorId { get; set; }
    public int? ModifierId { get; set; }

    public bool IsExpired() => DateTime.UtcNow >= Expires;
    public bool IsRevoked() => Revoked != null;
    public bool IsActive() => !IsRevoked() && !IsExpired();
}

public enum TokenRevokeReason
{
    Revoked = 1,
    Replaced = 2,
    Compromised = 3,
}