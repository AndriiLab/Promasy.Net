using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Requests;
using Promasy.Modules.Core.Responses;
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
         endpoints.MapGet(RoutePrefix, async (PagedRequest request, [FromRoute] int financeId, [FromServices] IFinanceFinanceSubDepartmentsRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(financeId, request);
                return Results.Json(list);
            })
            .WithValidator<PagedRequest>()
            .WithTags(Tag)
            .WithName("Get Finance sub-departments list")
            .RequireAuthorization()
            .Produces<PagedResponse<FinanceSubDepartmentDto>>();
         
         endpoints.MapGet($"{RoutePrefix}/{{subDepartmentId:int}}", async (int financeId, int subDepartmentId, [FromServices] IFinanceFinanceSubDepartmentsRepository repository) =>
             {
                 var fs = await repository.GetByFinanceSubDepartmentIdsAsync(financeId, subDepartmentId);
                 return fs is not null ? Results.Json(fs) : Results.NotFound();
             })
             .WithTags(Tag)
             .WithName("Get Finance sub-department by Id")
             .RequireAuthorization()
             .Produces<FinanceSubDepartmentDto>()
             .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateFinanceSubDepartmentRequest request, [FromRoute] int financeId, [FromServices] IFinanceFinanceSubDepartmentsRepository repository) =>
            {
                await repository.CreateAsync(new CreateFinanceSubDepartmentDto(request.FinanceSourceId, request.SubDepartmentId,
                    request.TotalEquipment, request.TotalMaterials, request.TotalServices));
                var fs = await repository.GetByFinanceSubDepartmentIdsAsync(request.FinanceSourceId, request.SubDepartmentId);

                return Results.Json(fs, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateFinanceSubDepartmentRequest>()
            .WithTags(Tag)
            .WithName("Create Finance sub-department")
            .RequireAuthorization()
            .Produces<FinanceSubDepartmentDto>(StatusCodes.Status201Created);

        endpoints.MapPut($"{RoutePrefix}/{{subDepartmentId:int}}",
                async ([FromBody] UpdateFinanceSubDepartmentRequest request, [FromRoute] int financeId, [FromRoute] int subDepartmentId, [FromServices] IFinanceFinanceSubDepartmentsRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.SubDepartmentId != subDepartmentId || request.FinanceSourceId != financeId)
                    {
                        return PromasyResults.ValidationError(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new UpdateFinanceSubDepartmentDto(request.Id, request.FinanceSourceId, request.SubDepartmentId,
                        request.TotalEquipment, request.TotalMaterials, request.TotalServices));

                    return Results.Ok(StatusCodes.Status202Accepted);
                })
            .WithValidator<UpdateFinanceSubDepartmentRequest>()
            .WithTags(Tag)
            .WithName("Update Finance sub-department")
            .RequireAuthorization()
            .Produces(StatusCodes.Status202Accepted);

        endpoints.MapDelete($"{RoutePrefix}/{{subDepartmentId:int}}", async ([FromRoute] int financeId, [FromRoute] int subDepartmentId, [FromServices] IFinanceFinanceSubDepartmentsRepository repository) =>
            {
                await repository.DeleteByFinanceSubDepartmentIdsAsync(financeId, subDepartmentId);
                return Results.Ok(StatusCodes.Status204NoContent);
            })
            .WithTags(Tag)
            .WithName("Delete Finance sub-department")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationErrorResponse>(StatusCodes.Status409Conflict);
        
        return endpoints;
    }
}