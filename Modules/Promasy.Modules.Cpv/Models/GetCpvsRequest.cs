using FluentValidation;
using Promasy.Core.Persistence;

namespace Promasy.Modules.Cpv.Models;

public record GetCpvsRequest(int? ParentId, int? Id, string? Search);

internal class GetCpvRequestValidator : AbstractValidator<GetCpvsRequest>
{
    public GetCpvRequestValidator()
    {
        RuleFor(r => r.Search)
            .MaximumLength(PersistenceConstant.FieldMedium);

        RuleFor(r => r.ParentId)
            .GreaterThan(0);
    }
}