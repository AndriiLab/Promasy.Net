using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Domain.Organizations;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Finances.Interfaces;

namespace Promasy.Modules.Finances.Models;

public record UpdateFinanceSubDepartmentRequest(int Id, int FinanceSourceId, int SubDepartmentId,
    decimal TotalEquipment, decimal TotalMaterials, decimal TotalServices);
    
internal class UpdateFinanceSubDepartmentRequestValidator : AbstractValidator<UpdateFinanceSubDepartmentRequest>
{
    public UpdateFinanceSubDepartmentRequestValidator(IFinanceFinanceSubDepartmentRules rules,
        IFinanceSourceRules financeSourceRules, IRules<SubDepartment> subDepartmentRules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Id)
            .MustAsync(rules.IsExistsAsync)
            .WithMessage(localizer["Item not exist"]);
        
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
                .MustAsync((r, t) => rules.CanBeAssignedAsEquipmentAsync(r.TotalEquipment, r.Id, r.FinanceSourceId, t))
                .WithName(nameof(CreateFinanceSubDepartmentRequest.TotalEquipment))
                .WithMessage(localizer["Cannot assign requested amount"]);
        });

        RuleFor(r => r.TotalMaterials)
            .GreaterThanOrEqualTo(0);
        
        When(r => r.TotalMaterials > 0, () =>
        {
            RuleFor(r => r)
                .MustAsync((r, t) => rules.CanBeAssignedAsMaterialsAsync(r.TotalMaterials, r.Id, r.FinanceSourceId, t))
                .WithName(nameof(CreateFinanceSubDepartmentRequest.TotalMaterials))
                .WithMessage(localizer["Cannot assign requested amount"]);
        });

        RuleFor(r => r.TotalServices)
            .GreaterThanOrEqualTo(0);
        
        When(r => r.TotalServices > 0, () =>
        {
            RuleFor(r => r)
                .MustAsync((r, t) => rules.CanBeAssignedAsServicesAsync(r.TotalServices, r.Id, r.FinanceSourceId, t))
                .WithName(nameof(CreateFinanceSubDepartmentRequest.TotalServices))
                .WithMessage(localizer["Cannot assign requested amount"]);
        });

        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsUniqueFinanceSubDepartmentAsync(r.Id, r.FinanceSourceId, r.SubDepartmentId, t))
            .WithName(nameof(CreateFinanceSubDepartmentRequest.SubDepartmentId))
            .WithMessage(localizer["Finance source for sub-department already exists"]);
    }
}