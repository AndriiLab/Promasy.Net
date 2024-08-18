using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Domain.Organizations;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Models;

public record UpdateOrganizationRequest(
    int Id,
    string Name,
    string Email,
    string Edrpou,
    string PhoneNumber,
    string? FaxNumber,
    string Country,
    string PostalCode,
    string Region,
    string City,
    int CityType,
    string Street,
    int StreetType,
    string BuildingNumber,
    string? InternalNumber) : IRequestWithPermissionValidation
{
    public int GetId() => Id;
}

internal class UpdateOrganizationRequestValidator : AbstractPermissionsValidator<UpdateOrganizationRequest>
{
    public UpdateOrganizationRequestValidator(IOrganizationRules rules, IStringLocalizer<SharedResource> localizer, IUserContext userContext)
        : base(rules, userContext, localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.Id)
            .MustAsync(rules.IsExistsAsync)
            .WithMessage(localizer["Item not exist"]);

        RuleFor(r => r)
            .MustAsync((r, t) => rules.IsNameUniqueAsync(r.Name, r.Id, t))
            .WithMessage(localizer["Name must be unique"]);
        
        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(PersistenceConstant.FieldMedium);
        
        RuleFor(r => r.Edrpou)
            .NotEmpty()
            .MaximumLength(20);
        
        RuleFor(r => r.PhoneNumber)
            .NotEmpty()
            .MaximumLength(30);
        
        RuleFor(r => r.FaxNumber)
            .MaximumLength(30);
        
        RuleFor(r => r.Country)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMini);
        
        RuleFor(r => r.PostalCode)
            .NotEmpty()
            .MaximumLength(10);
        
        RuleFor(r => r.Region)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMini);
        
        RuleFor(r => r.CityType)
            .Must(t => Enum.GetValues<CityType>().Any(e => (int) e == t))
            .WithMessage(localizer["City type does not exist"]);
        
        RuleFor(r => r.City)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMini);
        
        RuleFor(r => r.StreetType)
            .Must(t => Enum.GetValues<CityType>().Any(e => (int) e == t))
            .WithMessage(localizer["Street type does not exist"]);
        
        RuleFor(r => r.Street)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMini);
        
        RuleFor(r => r.BuildingNumber)
            .NotEmpty()
            .MaximumLength(10);
        
        RuleFor(r => r.InternalNumber)
            .MaximumLength(10);
    }
}