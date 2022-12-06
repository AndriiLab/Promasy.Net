using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Exceptions;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.OpenApi;
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

    public override IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
         endpoints.MapGet(RoutePrefix, async ([AsParameters] PagedRequest request, [FromRoute] int financeId, [FromServices] IFinanceFinanceSubDepartmentsRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(financeId, request);
                return TypedResults.Ok(list);
            })
            .WithValidator<PagedRequest>()
            .WithApiDescription(Tag, "GetFinanceSubDepartmentsList", "Get Finance sub-departments list")
            .RequireAuthorization();
         
         endpoints.MapGet("/api/departments/{departmentId:int}/sub-departments/{subDepartmentId:int}/finances", async ([AsParameters] PagedRequest request,
                 [FromRoute] int subDepartmentId, [FromRoute] int departmentId, [FromServices] IFinanceFinanceSubDepartmentsRepository repository) =>
             {
                 var list = await repository.GetPagedListBySubDepartmentAsync(subDepartmentId, request);
                 return TypedResults.Ok(list);
             })
             .WithValidator<PagedRequest>()
             .WithApiDescription(Tag, "GetFinanceSubDepartmentsBySubDepartmentId", "Get Finance sub-departments list by sub-department")
             .RequireAuthorization();
         
         endpoints.MapGet($"{RoutePrefix}/{{subDepartmentId:int}}", async ([FromRoute] int financeId, [FromRoute] int subDepartmentId,
                 [FromServices] IFinanceFinanceSubDepartmentsRepository repository) =>
             {
                 var fs = await repository.GetByFinanceSubDepartmentIdsAsync(financeId, subDepartmentId);
                 if (fs is null)
                 {
                     throw new ApiException(null, StatusCodes.Status404NotFound);
                 }
                 return TypedResults.Ok(fs);
             })
             .WithApiDescription(Tag, "GetFinanceSubDepartmentById", "Get Finance sub-department by Id")
             .RequireAuthorization()
             .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateFinanceSubDepartmentRequest request, [FromRoute] int financeId, [FromServices] IFinanceFinanceSubDepartmentsRepository repository) =>
            {
                await repository.CreateAsync(new CreateFinanceSubDepartmentDto(request.FinanceSourceId, request.SubDepartmentId,
                    request.TotalEquipment, request.TotalMaterials, request.TotalServices));
                var fs = await repository.GetByFinanceSubDepartmentIdsAsync(request.FinanceSourceId, request.SubDepartmentId);

                return TypedResults.Json(fs, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateFinanceSubDepartmentRequest>()
            .WithApiDescription(Tag, "CreateFinanceSubDepartment", "Create Finance sub-department")
            .RequireAuthorization();

        endpoints.MapPut($"{RoutePrefix}/{{subDepartmentId:int}}",
                async ([FromBody] UpdateFinanceSubDepartmentRequest request, [FromRoute] int financeId, [FromRoute] int subDepartmentId, [FromServices] IFinanceFinanceSubDepartmentsRepository repository,
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
            .WithApiDescription(Tag, "UpdateFinanceSubDepartment", "Update Finance sub-department")
            .RequireAuthorization();

        endpoints.MapDelete($"{RoutePrefix}/{{subDepartmentId:int}}", async ([FromRoute] int financeId, [FromRoute] int subDepartmentId, [FromServices] IFinanceFinanceSubDepartmentsRepository repository) =>
            {
                await repository.DeleteByFinanceSubDepartmentIdsAsync(financeId, subDepartmentId);
                return TypedResults.NoContent();
            })
            .WithApiDescription(Tag, "DeleteFinanceSubDepartment", "Delete Finance sub-department")
            .RequireAuthorization();
        
        return endpoints;
    }
}