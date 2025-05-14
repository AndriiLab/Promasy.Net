using System.Net;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Domain.Employees;
using Promasy.Modules.Core.Permissions;

namespace Promasy.Modules.Core.Validation;

public interface IPermissionsValidator<TModel> : IValidator<TModel> where TModel : IRequestWithPermissionValidation
{
    void SetRoleConditions(IReadOnlyCollection<(RoleName Role, PermissionCondition Condition)> rc);
}

public abstract class AbstractPermissionsValidator<TModel> : AbstractValidator<TModel>, IPermissionsValidator<TModel>
    where TModel : IRequestWithPermissionValidation
{
    private IReadOnlyCollection<(RoleName Role, PermissionCondition Condition)>? _roleConditions;

    protected AbstractPermissionsValidator(IPermissionRules rules, IUserContext userContext,
        IStringLocalizer<SharedResource> localizer)
    {
        RuleFor(x => x)
            .MustAsync((x, t) => HasPermissionsAsync(x, rules, _roleConditions, userContext, localizer, t))
            .WithMessage(localizer["Action not permitted"])
            .WithErrorCode(((int)HttpStatusCode.Forbidden).ToString());
    }

    private Task<bool> HasPermissionsAsync(TModel model, IPermissionRules rules,
        IReadOnlyCollection<(RoleName Role, PermissionCondition Condition)>? roleConditions, IUserContext userContext,
        IStringLocalizer<SharedResource> localizer, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(roleConditions);

        foreach (var (role, condition) in roleConditions.OrderBy(rc => (int)rc.Role))
        {
            if (!userContext.HasRoles((int)role))
                continue;

            switch (condition)
            {
                case PermissionCondition.Allowed:
                    return Task.FromResult(true);
                case PermissionCondition.SameOrganization:
                    return VerifySameOrganizationAsync(model, rules, userContext, cancellationToken);
                case PermissionCondition.SameDepartment:
                    return VerifySameDepartmentAsync(model, rules, userContext, cancellationToken);
                case PermissionCondition.SameSubDepartment:
                    return VerifySameSubDepartmentAsync(model, rules, userContext, cancellationToken);
                case PermissionCondition.SameUser:
                    return VerifySameUserAsync(model, rules, userContext, cancellationToken);
                default:
                    throw new ArgumentOutOfRangeException(nameof(condition), condition,
                        localizer["Permission condition is not supported"]);
            }
        }

        return Task.FromResult(false);
    }

    public void SetRoleConditions(IReadOnlyCollection<(RoleName Role, PermissionCondition Condition)> rc) =>
        _roleConditions = rc;

    protected virtual Task<bool> VerifySameOrganizationAsync(TModel model, IPermissionRules rules,
        IUserContext userContext, CancellationToken cancellationToken)
    {
        if (model is IRequestWithMultiplePermissionValidation m && rules is IPermissionRulesWithMultipleItems rm)
        {
            rm.IsSameOrganizationAsync(m.GetIds(), userContext.GetOrganizationId(), cancellationToken);
        }
        return rules.IsSameOrganizationAsync(model.GetId(), userContext.GetOrganizationId(), cancellationToken);
    }

    protected virtual Task<bool> VerifySameDepartmentAsync(TModel model, IPermissionRules rules,
        IUserContext userContext, CancellationToken cancellationToken)
    {
        if (model is IRequestWithMultiplePermissionValidation m && rules is IPermissionRulesWithMultipleItems rm)
        {
            rm.IsSameDepartmentAsync(m.GetIds(), userContext.GetDepartmentId(), cancellationToken);
        }
        return rules.IsSameDepartmentAsync(model.GetId(), userContext.GetDepartmentId(), cancellationToken);
    }

    protected virtual Task<bool> VerifySameSubDepartmentAsync(TModel model, IPermissionRules rules,
        IUserContext userContext, CancellationToken cancellationToken)
    {
        if (model is IRequestWithMultiplePermissionValidation m && rules is IPermissionRulesWithMultipleItems rm)
        {
            rm.IsSameSubDepartmentAsync(m.GetIds(), userContext.GetSubDepartmentId(), cancellationToken);
        }
        return rules.IsSameSubDepartmentAsync(model.GetId(), userContext.GetSubDepartmentId(), cancellationToken);
    }

    protected virtual Task<bool> VerifySameUserAsync(TModel model, IPermissionRules rules, IUserContext userContext,
        CancellationToken cancellationToken)
    {
        if (model is IRequestWithMultiplePermissionValidation m && rules is IPermissionRulesWithMultipleItems rm)
        {
            rm.IsSameUserAsync(m.GetIds(), userContext.GetId(), cancellationToken);
        }
        return rules.IsSameUserAsync(model.GetId(), userContext.GetId(), cancellationToken);
    }
}