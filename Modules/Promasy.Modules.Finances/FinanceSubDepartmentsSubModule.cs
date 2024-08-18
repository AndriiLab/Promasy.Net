using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Domain.Employees;
using Promasy.Modules.Core.Exceptions;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.OpenApi;
using Promasy.Modules.Core.Permissions;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Finances.Dtos;
using Promasy.Modules.Finances.Interfaces;
using Promasy.Modules.Finances.Models;

namespace Promasy.Modules.Finances;

internal class FinanceSubDepartmentsSubModule : SubModule
{
    public FinanceSubDepartmentsSubModule(string parentRoutePrefix) 
        : base(parentRoutePrefix, "/sub-departments")
    {
    }

    public override string Tag { get; } = "Finance sub-department";

    public override IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        return builder;
    }

    public override WebApplication MapEndpoints(WebApplication app)
    {
         app.MapGet(RoutePrefix, async ([AsParameters] PagedRequest request, [FromRoute] int financeId, [FromServices] IFinanceSubDepartmentsRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(financeId, request);
                return TypedResults.Ok(list);
            })
            .WithValidator<PagedRequest>()
            .WithApiDescription(Tag, "GetFinanceSubDepartmentsList", "Get Finance sub-departments list")
            .RequireAuthorization();
         
         app.MapGet("/api/departments/{departmentId:int}/sub-departments/{subDepartmentId:int}/finances", async ([AsParameters] GetFinanceSubDepartmentsPagedRequest request,
                 [FromServices] IFinanceSubDepartmentsRepository repository) =>
             {
                 var list = await repository.GetPagedListBySubDepartmentAsync(request);
                 return TypedResults.Ok(list);
             })
             .WithAuthorizationAndValidation<GetFinanceSubDepartmentsPagedRequest>(app, Tag, "Get Finance sub-departments list by sub-department", PermissionTag.List,
                 Enum.GetValues<RoleName>().Select(r =>
                 (r, r switch
                     {
                         RoleName.User => PermissionCondition.SameDepartment,
                         RoleName.PersonallyLiableEmployee => PermissionCondition.SameDepartment,
                         RoleName.HeadOfDepartment => PermissionCondition.SameDepartment,
                         _ => PermissionCondition.None
                     })).ToArray());
         
         app.MapGet($"{RoutePrefix}/{{subDepartmentId:int}}", async ([AsParameters] GetFinanceSubDepartmentRequest request,
                 [FromServices] IFinanceSubDepartmentsRepository repository) =>
             {
                 var fs = await repository.GetByFinanceSubDepartmentIdsAsync(request.FinanceId, request.SubDepartmentId);
                 if (fs is null)
                 {
                     throw new ApiException(null, StatusCodes.Status404NotFound);
                 }
                 return TypedResults.Ok(fs);
             })
             .Produces(StatusCodes.Status404NotFound)
             .WithAuthorizationAndValidation<GetFinanceSubDepartmentRequest>(app, Tag, "Get Finance sub-department by Id", PermissionTag.Get,
                 Enum.GetValues<RoleName>().Select(r =>
                 (r, r switch
                 {
                     RoleName.User => PermissionCondition.SameDepartment,
                     RoleName.PersonallyLiableEmployee => PermissionCondition.SameDepartment,
                     RoleName.HeadOfDepartment => PermissionCondition.SameDepartment,
                     _ => PermissionCondition.None
                 })).ToArray());


         app.MapPost(RoutePrefix,
                 async ([FromBody] CreateFinanceSubDepartmentRequest request, [FromRoute] int financeId,
                     [FromServices] IFinanceSubDepartmentsRepository repository) =>
                 {
                     await repository.CreateAsync(new CreateFinanceSubDepartmentDto(request.FinanceSourceId,
                         request.SubDepartmentId,
                         request.TotalEquipment, request.TotalMaterials, request.TotalServices));
                     var fs = await repository.GetByFinanceSubDepartmentIdsAsync(request.FinanceSourceId,
                         request.SubDepartmentId);

                     return TypedResults.Json(fs, statusCode: StatusCodes.Status201Created);
                 })
             .WithValidator<CreateFinanceSubDepartmentRequest>()
             .WithAuthorization(app, Tag,"Create Finance sub-department", PermissionTag.Create,
                 RoleName.Administrator,
                 RoleName.ChiefAccountant,
                 RoleName.ChiefEconomist);

        app.MapPut($"{RoutePrefix}/{{subDepartmentId:int}}",
                async ([FromBody] UpdateFinanceSubDepartmentRequest request, [FromRoute] int financeId, [FromRoute] int subDepartmentId, [FromServices] IFinanceSubDepartmentsRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.SubDepartmentId != subDepartmentId || request.FinanceSourceId != financeId)
                    {
                        throw new ApiException(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new UpdateFinanceSubDepartmentDto(request.Id, request.FinanceSourceId, request.SubDepartmentId,
                        request.TotalEquipment, request.TotalMaterials, request.TotalServices));

                    return TypedResults.Accepted(string.Empty);
                })
            .WithValidator<UpdateFinanceSubDepartmentRequest>()
            .WithAuthorization(app, Tag, "Update Finance sub-department", PermissionTag.Update,
                RoleName.Administrator,
                RoleName.ChiefAccountant,
                RoleName.ChiefEconomist);

        app.MapDelete($"{RoutePrefix}/{{subDepartmentId:int}}", async ([AsParameters] DeleteFinanceSubDepartmentRequest request, [FromServices] IFinanceSubDepartmentsRepository repository) =>
            {
                await repository.DeleteByFinanceSubDepartmentIdsAsync(request.FinanceId, request.SubDepartmentId);
                return TypedResults.NoContent();
            })
            .WithAuthorization(app, Tag, "Delete Finance sub-department", PermissionTag.Delete,
                RoleName.Administrator,
                RoleName.ChiefAccountant,
                RoleName.ChiefEconomist);
        
        return app;
    }
}