using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Domain.Employees;
using Promasy.Domain.Organizations;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Employees.Dtos;
using Promasy.Modules.Employees.Interfaces;
using Riok.Mapperly.Abstractions;

namespace Promasy.Modules.Employees.Models;

public record UpdateEmployeeRequest(int Id, string FirstName, string? MiddleName, string LastName,
    string Email, string PrimaryPhone, string? ReservePhone, int SubDepartmentId,
    RoleName[] Roles) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

[Mapper]
internal static partial class UpdateEmployeeRequestMapper
{
    public static partial UpdateEmployeeDto MapFromSource(UpdateEmployeeRequest src);
}

internal class UpdateEmployeeRequestValidator : AbstractPermissionsValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator(IEmployeeRules employeeRules, IRules<SubDepartment> subDepartmentRules,
         IUserContext userContext, IStringLocalizer<SharedResource> localizer) : base(employeeRules, userContext, localizer)
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
            .MustAsync(subDepartmentRules.IsExistsAsync)
            .WithMessage(localizer["Sub-department not exist"]);
        
        RuleFor(r => r.Roles)
            .Must(r => r.Length == 1)
            .WithMessage(localizer["Employee must have one role"]);

        RuleFor(r => r)
            .MustAsync((r,t) => employeeRules.IsEmailUniqueAsync(r.Email, r.Id, t))
            .WithMessage(localizer["Email must be unique"])
            .WithName(nameof(UpdateEmployeeRequest.Email));

        RuleFor(r => r)
            .MustAsync((r, t) => employeeRules.IsPhoneUniqueAsync(r.PrimaryPhone, r.Id, t))
            .WithMessage(localizer["Phone must be unique"])
            .WithName(nameof(UpdateEmployeeRequest.PrimaryPhone));
            
        RuleFor(r => r)
            .MustAsync((r,t) => employeeRules.CanHaveRolesAsync(r.Roles, r.Id, t))
            .WithMessage(localizer["You can assign only User role to the employee"])
            .WithName(nameof(UpdateEmployeeRequest.Roles));
    }
}