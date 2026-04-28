using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Domain.Organizations;
using Promasy.Modules.Finances.Interfaces;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;

namespace Promasy.Modules.Finances.Models;

public record CreateFinanceSubDepartmentRequest(int FinanceSourceId, int SubDepartmentId,
    decimal TotalEquipment, decimal TotalMaterials, decimal TotalServices) : IRequestWithPermissionValidation
{
    public int GetId() => throw new NotSupportedException();
}

internal class CreateFinanceSubDepartmentRequestValidator : AbstractPermissionsValidator<CreateFinanceSubDepartmentRequest>
{
    public CreateFinanceSubDepartmentRequestValidator(IFinanceSubDepartmentRules rules,
        IFinanceSourceRules financeSourceRules, IRules<SubDepartment> subDepartmentRules, IUserContext userContext, IStringLocalizer<SharedResource> localizer) : base(rules, userContext, localizer)
    {
        RuleFor(r => r.FinanceSourceId)
            .MustAsync(financeSourceRules.IsExistsAsync)
            .WithMessage(localizer["Finance source not exists"]);
        
        RuleFor(r => r.SubDepartmentId)
            .MustAsync(subDepartmentRules.IsExistsAsync)
            .WithMessage(localizer["Sub-department not exist"]);

        RuleFor(r => r.TotalEquipment)
            .GreaterThanOrEqualTo(0);
        
        When(r => r.TotalEquipment > 0, () =>
        {
            RuleFor(r => r)
                .MustAsync((r, t) => rules.CanBeAssignedAsEquipmentAsync(r.TotalEquipment, r.FinanceSourceId, t))
                .WithName(nameof(CreateFinanceSubDepartmentRequest.TotalEquipment))
                .WithMessage(localizer["Cannot assign requested amount"]);
        });

        RuleFor(r => r.TotalMaterials)
            .GreaterThanOrEqualTo(0);
        
        When(r => r.TotalMaterials > 0, () =>
        {
            RuleFor(r => r)
                .MustAsync((r, t) => rules.CanBeAssignedAsMaterialsAsync(r.TotalMaterials, r.FinanceSourceId, t))
                .WithName(nameof(CreateFinanceSubDepartmentRequest.TotalMaterials))
                .WithMessage(localizer["Cannot assign requested amount"]);
        });

        RuleFor(r => r.TotalServices)
            .GreaterThanOrEqualTo(0);
        
        When(r => r.TotalServices > 0, () =>
        {
            RuleFor(r => r)
                .MustAsync((r, t) => rules.CanBeAssignedAsServicesAsync(r.TotalServices, r.FinanceSourceId, t))
                .WithName(nameof(CreateFinanceSubDepartmentRequest.TotalServices))
                .WithMessage(localizer["Cannot assign requested amount"]);
        });

        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsUniqueFinanceSubDepartmentAsync(r.FinanceSourceId, r.SubDepartmentId, t))
            .WithName(nameof(CreateFinanceSubDepartmentRequest.SubDepartmentId))
            .WithMessage(localizer["Finance source for sub-department already exists"]);
    }
}