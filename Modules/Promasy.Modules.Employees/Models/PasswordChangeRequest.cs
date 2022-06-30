using FluentValidation;
using Promasy.Core.Persistence;

namespace Promasy.Modules.Employees.Models;

public record PasswordChangeRequest(string Password);


public class PasswordChangeRequestValidator : AbstractValidator<PasswordChangeRequest>
{
    public PasswordChangeRequestValidator()
    {
        RuleFor(m => m.Password)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);
    }
}