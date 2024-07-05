using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Promasy.Core.Persistence;

namespace Promasy.Modules.Employees.Models;

public record PasswordChangeRequest(string Password, [FromRoute] int Id);


internal class PasswordChangeRequestValidator : AbstractValidator<PasswordChangeRequest>
{
    public PasswordChangeRequestValidator()
    {
        RuleFor(m => m.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(PersistenceConstant.FieldMedium);
    }
}