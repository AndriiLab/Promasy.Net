using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Domain.Finances;
using Promasy.Domain.Manufacturers;
using Promasy.Domain.Orders;
using Promasy.Domain.Suppliers;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Orders.Interfaces;

namespace Promasy.Modules.Orders.Models;

public record UpdateOrderRequest(int Id, string Description, string? CatNum, decimal OnePrice, int Amount,
    OrderType Type, string? Kekv, DateOnly? ProcurementStartDate, int UnitId, int CpvId,
    int FinanceSubDepartmentId, int ManufacturerId, int SupplierId, int ReasonId);
    
internal class UpdateOrderRequestValidator : AbstractValidator<UpdateOrderRequest>
{
    public UpdateOrderRequestValidator(IStringLocalizer<SharedResource> localizer, IRules<Unit> unitRules,
        IOrderRules rules, IRules<FinanceSubDepartment> financeSubDepartmentRules, IRules<Manufacturer> manufacturerRules,
        IRules<Supplier> supplierRules, IRules<ReasonForSupplierChoice> reasonForSupplierChoiceRules)
    {
        RuleFor(r => r.Id)
            .MustAsync(rules.IsExistsAsync)
            .WithMessage(localizer["Order not exists"]);
        
        RuleFor(r => r.Description)
            .NotEmpty()
            .MinimumLength(3)
            .MinimumLength(PersistenceConstant.FieldLarge);
        
        RuleFor(r => r.CatNum)
            .MinimumLength(PersistenceConstant.FieldMedium);
        
        RuleFor(r => r.OnePrice)
            .GreaterThan(0);
        
        RuleFor(r => r.Amount)
            .GreaterThan(0);
        
        RuleFor(r => r.Type)
            .Must(t => Enum.GetValues<OrderType>().Any(e => e == t))
            .WithMessage(localizer["Order type not exists"]);
        
        RuleFor(r => r.Kekv)
            .MinimumLength(PersistenceConstant.FieldMini);
        
        RuleFor(r => r.UnitId)
            .MustAsync(unitRules.IsExistsAsync)
            .WithMessage(localizer["Unit not exists"]);
        
        RuleFor(r => r.CpvId)
            .MustAsync(rules.IsCpvCanBeUsedAsync)
            .WithMessage(localizer["CPV not exists or it cannot be used"]);
        
        RuleFor(r => r.FinanceSubDepartmentId)
            .MustAsync(financeSubDepartmentRules.IsExistsAsync)
            .WithMessage(localizer["Finance source not exists"]);
        
        RuleFor(r => r.ManufacturerId)
            .MustAsync(manufacturerRules.IsExistsAsync)
            .WithMessage(localizer["Manufacturer not exists"]);        
        
        RuleFor(r => r.SupplierId)
            .MustAsync(supplierRules.IsExistsAsync)
            .WithMessage(localizer["Supplier not exists"]);
        
        RuleFor(r => r.ReasonId)
            .MustAsync(reasonForSupplierChoiceRules.IsExistsAsync)
            .WithMessage(localizer["Reason for supplier choice not exists"]);

        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsSufficientFundsAsync(r.FinanceSubDepartmentId, r.Id, r.Amount * r.OnePrice, r.Type, t))
            .WithMessage(localizer["Insufficient funds for the order"]);
    }
}