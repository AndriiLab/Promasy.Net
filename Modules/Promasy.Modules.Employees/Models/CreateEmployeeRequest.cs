using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Core.UserContext;
using Promasy.Domain.Employees;
using Promasy.Modules.Core.Rules;

namespace Promasy.Modules.Employees.Models;

public record CreateEmployeeRequest(string FirstName, string? MiddleName, string LastName,
    string Email, string PrimaryPhone, string? ReservePhone, string UserName, string Password,
    int SubDepartmentId, RoleName[] Roles);

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator(IEmployeesRules employeesRules, ISubDepartmentsRules subDepartmentsRules,
         IUserContext userContext, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.FirstName)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.MiddleName)
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.LastName)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.Email)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium)
            .EmailAddress()
            .MustAsync(employeesRules.IsEmailUniqueAsync)
            .WithMessage(localizer["Email must be unique"]);

        RuleFor(r => r.PrimaryPhone)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium)
            .MustAsync(employeesRules.IsPhoneUniqueAsync)
            .WithMessage(localizer["Phone must be unique"]);

        RuleFor(r => r.UserName)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium)
            .MustAsync(employeesRules.IsUserNameUniqueAsync)
            .WithMessage(localizer["User name must be unique"]);

        When(r => !string.IsNullOrEmpty(r.ReservePhone), () =>
        {
            RuleFor(r => r.ReservePhone)
                .MaximumLength(PersistenceConstant.FieldMedium)
                .MustAsync(employeesRules.IsPhoneUniqueAsync)
                .WithMessage(localizer["Reserve phone must be unique"]);
        });

        RuleFor(r => r.SubDepartmentId)
            .MustAsync(subDepartmentsRules.IsExistAsync)
            .WithMessage(localizer["Sub-department not exist"]);
        
        RuleFor(m => m.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.Roles)
            .Must(r => r.Length == 1)
            .WithMessage(localizer["Employee must have one role"])
            .Must(employeesRules.CanHaveRoles)
            .WithMessage(localizer["You can assign only User role to the employee"]);
    }
}