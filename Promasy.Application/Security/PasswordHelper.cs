using System.Security.Cryptography;
using System.Text;

namespace Promasy.Application.Security;

public static class PasswordHelper
{
    public static string Hash(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public static bool Validate(string password, string hash, long? salt)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        if (salt.HasValue)
        {
            // fallback to old validation scheme
#pragma warning disable CS0618 // Type or member is obsolete
            var calculatedHash = CalculateOldHash(password, salt.Value);
#pragma warning restore CS0618 // Type or member is obsolete
            return string.Equals(calculatedHash, hash);
        }

        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
    
    [Obsolete("Used for compatibility with old version passwords")]
    private static string CalculateOldHash(string password, long salt)
    {
        var hasher = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password).Concat(BitConverter.GetBytes(salt).Reverse()).ToArray();
        var hash = hasher.ComputeHash(bytes);
        var sb = new StringBuilder();
        foreach (var b in hash)
        {
            sb.Append($"{b:x2}".ToUpper());
        }
        return sb.ToString();
    }
}