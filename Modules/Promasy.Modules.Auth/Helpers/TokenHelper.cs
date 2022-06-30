using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Promasy.Core.Extensions;
using Promasy.Modules.Auth.Dtos;

namespace Promasy.Modules.Auth.Helpers;

internal static class TokenHelper
{
    public static string GenerateJwtToken(EmployeeDto employee, string secret, int validityMinutes)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(nameof(employee.Id).ToCamelCase(), employee.Id.ToString()),
                new Claim(nameof(employee.FirstName).ToCamelCase(), employee.FirstName),
                new Claim(nameof(employee.MiddleName).ToCamelCase(), employee.MiddleName),
                new Claim(nameof(employee.LastName).ToCamelCase(), employee.LastName),
                new Claim(nameof(employee.Email).ToCamelCase(), employee.Email),
                new Claim(nameof(employee.Organization).ToCamelCase(), employee.Organization),
                new Claim(nameof(employee.OrganizationId).ToCamelCase(), employee.OrganizationId.ToString()),
                new Claim(nameof(employee.Department).ToCamelCase(), employee.Department),
                new Claim(nameof(employee.DepartmentId).ToCamelCase(), employee.DepartmentId.ToString()),
                new Claim(nameof(employee.SubDepartment).ToCamelCase(), employee.SubDepartment),
                new Claim(nameof(employee.SubDepartmentId).ToCamelCase(), employee.SubDepartmentId.ToString()),
                new Claim(nameof(employee.Roles).ToCamelCase(), string.Join(',', employee.Roles)),
            }),
            Expires = DateTime.UtcNow.AddMinutes(validityMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static string GenerateRefreshToken(int employeeId, string secret, int validityMinutes)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", employeeId.ToString()),
            }),
            Expires = DateTime.UtcNow.AddMinutes(validityMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public static int? ValidateAndGetUserId(string token, string secret)
    {
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = validatedToken as JwtSecurityToken;

            return int.TryParse(jwtToken?.Claims.First(x => x.Type == "id").Value, out var id) ? id : null;
        }
        catch
        {
            return null;
        }
    }
}