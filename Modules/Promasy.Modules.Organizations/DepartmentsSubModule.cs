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
using Promasy.Modules.Core.Rules;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Organizations.Dtos;
using Promasy.Modules.Organizations.Interfaces;
using Promasy.Modules.Organizations.Models;

namespace Promasy.Modules.Organizations;

internal class DepartmentsSubModule : SubModule
{
    private readonly SubDepartmentsSubModule _subDepartmentsSubModule;
    public override string Tag { get; } = "Department";
    public DepartmentsSubModule(string parentRoutePrefix) : base(parentRoutePrefix, "/departments")
    {
        _subDepartmentsSubModule = new SubDepartmentsSubModule($"{RoutePrefix}/{{departmentId:int}}");
    }

    public override IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        _subDepartmentsSubModule.RegisterServices(builder, configuration);
        return builder;
    }

    public override IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(RoutePrefix, async (PagedRequest request, [FromRoute] int organizationId, [FromServices] IDepartmentsRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return Results.Json(list);
            })
            .WithValidator<PagedRequest>()
            .WithTags(Tag)
            .WithName("Get Departments list")
            .RequireAuthorization()
            .Produces<PagedResponse<DepartmentDto>>();

        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromRoute]int id, [FromRoute] int organizationId, [FromServices] IDepartmentsRepository repository) =>
            {
                var item = await repository.GetByIdAsync(id);
                return item is not null ? Results.Json(item) : Results.NotFound();
            })
            .WithTags(Tag)
            .WithName("Get Department by Id")
            .RequireAuthorization()
            .Produces<DepartmentDto>()
            .Produces(StatusCodes.Status404NotFound);

        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateDepartmentRequest request, [FromRoute] int organizationId, [FromServices] IDepartmentsRepository repository,
                [FromServices] IStringLocalizer<SharedResource> localizer) =>
            {
                if (organizationId != request.OrganizationId)
                {
                    return PromasyResults.ValidationError(localizer["Incorrect organization id"]);
                }

                var id = await repository.CreateAsync(new DepartmentDto(0, request.Name, request.OrganizationId));
                var unit = await repository.GetByIdAsync(id);

                return Results.Json(unit, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateDepartmentRequest>()
            .WithTags(Tag)
            .WithName("Create Department")
            .RequireAuthorization()
            .Produces<DepartmentDto>(StatusCodes.Status201Created);

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateDepartmentRequest request, [FromRoute] int id, [FromRoute] int organizationId, [FromServices] IDepartmentsRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (organizationId != request.OrganizationId)
                    {
                        return PromasyResults.ValidationError(localizer["Incorrect organization id"]);
                    }
                    
                    if (request.Id != id)
                    {
                        return PromasyResults.ValidationError(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new DepartmentDto(request.Id, request.Name, request.OrganizationId));

                    return Results.Ok(StatusCodes.Status202Accepted);
                })
            .WithValidator<UpdateDepartmentRequest>()
            .WithTags(Tag)
            .WithName("Update Department")
            .RequireAuthorization()
            .Produces(StatusCodes.Status202Accepted);

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromRoute] int organizationId, [FromServices] IDepartmentsRepository repository,
                [FromServices] IDepartmentsRules rules, [FromServices] IStringLocalizer<SharedResource> localizer) =>
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
                    return PromasyResults.ValidationError(localizer["Department has subdepartments"],
                        StatusCodes.Status409Conflict);
                }

                await repository.DeleteByIdAsync(id);
                return Results.Ok(StatusCodes.Status204NoContent);
            })
            .WithTags(Tag)
            .WithName("Delete Department by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationErrorResponse>(StatusCodes.Status409Conflict);
        
        _subDepartmentsSubModule.MapEndpoints(endpoints);
        return endpoints;
    }
}