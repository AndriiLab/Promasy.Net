namespace Promasy.Modules.Auth.Dtos;

internal record EmployeePasswordDto(int Id, string PasswordHash, long? PasswordSalt);