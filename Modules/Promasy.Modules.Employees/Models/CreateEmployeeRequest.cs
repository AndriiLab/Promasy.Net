using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Domain.Employees;
using Promasy.Domain.Organizations;
using Promasy.Modules.Core.Mapper;
using Promasy.Modules.Employees.Dtos;
using Promasy.Modules.Employees.Interfaces;
using Riok.Mapperly.Abstractions;

namespace Promasy.Modules.Employees.Models;

public record CreateEmployeeRequest(string FirstName, string? MiddleName, string LastName,
    string Email, string PrimaryPhone, string? ReservePhone, string UserName, string Password,
    int SubDepartmentId, RoleName[] Roles);

[Mapper]
internal partial class CreateEmployeeRequestMapper : IMapper<CreateEmployeeRequest, CreateEmployeeDto>
{
    public partial CreateEmployeeDto MapFromSource(CreateEmployeeRequest src);
}

internal class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
{
    public CreateEmployeeRequestValidator(IEmployeeRules employeeRules, IRules<SubDepartment> subDepartmentRules,
        IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.FirstName)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMini);

        RuleFor(r => r.MiddleName)
            .MaximumLength(PersistenceConstant.FieldMini);

        RuleFor(r => r.LastName)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMini);

        RuleFor(r => r.Email)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMini)
            .EmailAddress()
            .MustAsync(employeeRules.IsEmailUniqueAsync)
            .WithMessage(localizer["Email must be unique"]);

        RuleFor(r => r.PrimaryPhone)
            .NotEmpty()
            .MaximumLength(30)
            .MustAsync(employeeRules.IsPhoneUniqueAsync)
            .WithMessage(localizer["Phone must be unique"]);

        RuleFor(r => r.UserName)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMini)
            .MustAsync(employeeRules.IsUserNameUniqueAsync)
            .WithMessage(localizer["User name must be unique"]);

        When(r => !string.IsNullOrEmpty(r.ReservePhone), () =>
        {
            RuleFor(r => r.ReservePhone)
                .MaximumLength(30);
        });

        RuleFor(r => r.SubDepartmentId)
            .MustAsync(subDepartmentRules.IsExistsAsync)
            .WithMessage(localizer["Sub-department not exist"]);
        
        RuleFor(m => m.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(PersistenceConstant.FieldMini);

        RuleFor(r => r.Roles)
            .Must(r => r.Length == 1)
            .WithMessage(localizer["Employee must have one role"])
            .Must(employeeRules.CanHaveRoles)
            .WithMessage(localizer["You can assign only User role to the employee"]);
    }
}