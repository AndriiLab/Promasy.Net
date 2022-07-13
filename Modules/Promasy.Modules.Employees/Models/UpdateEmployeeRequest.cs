﻿using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Core.UserContext;
using Promasy.Domain.Employees;
using Promasy.Modules.Core.Rules;

namespace Promasy.Modules.Employees.Models;

public record UpdateEmployeeRequest(int Id, string FirstName, string? MiddleName, string LastName,
    string Email, string PrimaryPhone, string? ReservePhone, int SubDepartmentId,
    RoleName[] Roles);
    
    
public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator(IEmployeesRules employeesRules, ISubDepartmentsRules subDepartmentsRules,
         IUserContext userContext, IStringLocalizer<SharedResource> localizer)
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
            .EmailAddress();

        RuleFor(r => r.PrimaryPhone)
            .NotEmpty()
            .MaximumLength(30);

        When(r => !string.IsNullOrEmpty(r.ReservePhone), () =>
        {
            RuleFor(r => r.ReservePhone)
                .MaximumLength(30);
        });

        RuleFor(r => r.SubDepartmentId)
            .MustAsync(subDepartmentsRules.IsExistAsync)
            .WithMessage(localizer["Sub-department not exist"]);
        
        RuleFor(r => r.Roles)
            .Must(r => r.Length == 1)
            .WithMessage(localizer["Employee must have one role"]);

        RuleFor(r => r)
            .MustAsync((r,t) => employeesRules.IsEmailUniqueAsync(r.Email, r.Id, t))
            .WithMessage(localizer["Email must be unique"])
            .MustAsync((r, t) => employeesRules.IsPhoneUniqueAsync(r.PrimaryPhone, r.Id, t))
            .WithMessage(localizer["Phone must be unique"])
            .MustAsync((r,t) => employeesRules.CanHaveRolesAsync(r.Roles, r.Id, t))
            .WithMessage(localizer["You can assign only User role to the employee"]);
    }
}