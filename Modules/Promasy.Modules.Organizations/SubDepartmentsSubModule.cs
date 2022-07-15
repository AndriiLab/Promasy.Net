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
using Promasy.Modules.Organizations.Dtos;
using Promasy.Modules.Organizations.Interfaces;
using Promasy.Modules.Organizations.Models;

namespace Promasy.Modules.Organizations;

internal class SubDepartmentsSubModule : SubModule
{
    public override string Tag { get; } = "Sub-department";
    
    public SubDepartmentsSubModule(string parentRoutePrefix) : base(parentRoutePrefix, "/sub-departments")
    {
    }

    public override IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        return builder;
    }

    public override IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(RoutePrefix, async (PagedRequest request, [FromRoute] int organizationId, [FromRoute] int departmentId, [FromServices] ISubDepartmentsRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(departmentId, request);
                return Results.Json(list);
            })
            .WithValidator<PagedRequest>()
            .WithTags(Tag)
            .WithName("Get SubDepartments list")
            .RequireAuthorization()
            .Produces<PagedResponse<SubDepartmentDto>>();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromRoute] int organizationId, 
                [FromRoute] int departmentId, [FromServices] ISubDepartmentsRepository repository) =>
            {
                var unit = await repository.GetByIdAsync(id);
                return unit is not null ? Results.Json(unit) : Results.NotFound();
            })
            .WithTags(Tag)
            .WithName("Get SubDepartment by Id")
            .RequireAuthorization()
            .Produces<SubDepartmentDto>()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateSubDepartmentRequest request, [FromRoute] int organizationId,
                [FromRoute] int departmentId,  [FromServices] ISubDepartmentsRepository repository,
                [FromServices] IStringLocalizer<SharedResource> localizer) =>
            {
                if (organizationId != request.OrganizationId)
                {
                    return PromasyResults.ValidationError(localizer["Incorrect organization id"]);
                }
                
                if (departmentId != request.DepartmentId)
                {
                    return PromasyResults.ValidationError(localizer["Incorrect department id"]);
                }
                
                var id = await repository.CreateAsync(new SubDepartmentDto(0, request.Name, request.DepartmentId));
                var unit = await repository.GetByIdAsync(id);

                return Results.Json(unit, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateSubDepartmentRequest>()
            .WithTags(Tag)
            .WithName("Create SubDepartment")
            .RequireAuthorization()
            .Produces<SubDepartmentDto>(StatusCodes.Status201Created);

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateSubDepartmentRequest request, [FromRoute] int id, [FromRoute] int organizationId,
                    [FromRoute] int departmentId, [FromServices] ISubDepartmentsRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (departmentId != request.DepartmentId)
                    {
                        return PromasyResults.ValidationError(localizer["Incorrect department id"]);
                    }
                    
                    if (request.Id != id)
                    {
                        return PromasyResults.ValidationError(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new SubDepartmentDto(request.Id, request.Name, request.DepartmentId));

                    return Results.Ok(StatusCodes.Status202Accepted);
                })
            .WithValidator<UpdateSubDepartmentRequest>()
            .WithTags(Tag)
            .WithName("Update SubDepartment")
            .RequireAuthorization()
            .Produces(StatusCodes.Status202Accepted);

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromRoute] int organizationId,
                [FromRoute] int departmentId, [FromServices] ISubDepartmentsRepository repository,
                [FromServices] ISubDepartmentRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
            {
                var isEditable = rules.IsEditable(id);
                if (!isEditable)
                {
                    return PromasyResults.ValidationError(localizer["You cannot perform this action"],
                        StatusCodes.Status409Conflict);
                }
                
                var isUsed = await rules.IsUsedAsync(id, CancellationToken.None);
                if (isUsed)
                {
                    return PromasyResults.ValidationError(localizer["Unit already associated with order"],
                        StatusCodes.Status409Conflict);
                }

                await repository.DeleteByIdAsync(id);
                return Results.Ok(StatusCodes.Status204NoContent);
            })
            .WithTags(Tag)
            .WithName("Delete SubDepartment by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationErrorResponse>(StatusCodes.Status409Conflict);
        
        return endpoints;
    }
}