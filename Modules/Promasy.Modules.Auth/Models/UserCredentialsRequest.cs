using FluentValidation;
using Promasy.Core.Persistence;

namespace Promasy.Modules.Auth.Models;

public record UserCredentialsRequest(string User, string Password);

public class UserCredentialsRequestValidator : AbstractValidator<UserCredentialsRequest>
{
    public UserCredentialsRequestValidator()
    {
        RuleFor(m => m.User)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);
        
        RuleFor(m => m.Password)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);
    }
}