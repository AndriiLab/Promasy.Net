﻿using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Core.Persistence;
using Promasy.Core.Resources;
using Promasy.Domain.Organizations;
using Promasy.Modules.Organizations.Interfaces;

namespace Promasy.Modules.Organizations.Models;

public record CreateOrganizationRequest(
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
    string? InternalNumber);

internal class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest>
{
    public CreateOrganizationRequestValidator(IOrganizationRules rules, IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(PersistenceConstant.FieldMedium)
            .MustAsync(rules.IsNameUniqueAsync)
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