using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Domain.Finances;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Finances.Dtos;
using Promasy.Modules.Finances.Interfaces;
using Promasy.Modules.Finances.Models;

namespace Promasy.Modules.Finances;

public class FinancesModule : IModule
{
    private readonly FinanceSubDepartmentsSubModule _fsSubModule;
    public string Tag { get; } = "Finance";
    public string RoutePrefix { get; } = "/api/finances";

    public FinancesModule()
    {
        _fsSubModule = new FinanceSubDepartmentsSubModule($"{RoutePrefix}/{{financeId:int}}");
    }
    
    public IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        _fsSubModule.RegisterServices(builder, configuration);
    
        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{RoutePrefix}/all-fund-types", ([FromServices] IStringLocalizer<FinanceFundType> localizer) =>
            {
                return Results.Json(Enum.GetValues<FinanceFundType>()
                    .Select(r => new SelectItem<int>((int) r, localizer[r.ToString()])));
            })
            .WithTags(Tag)
            .WithName("Get available fund types")
            .Produces<SelectItem<int>[]>();
        
        endpoints.MapGet(RoutePrefix, async (FinanceSourcesPagedRequest request, [FromServices] IFinanceSourcesRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return Results.Json(list);
            })
            .WithValidator<FinanceSourcesPagedRequest>()
            .WithTags(Tag)
            .WithName("Get Finance sources list")
            .RequireAuthorization()
            .Produces<PagedResponse<FinanceSourceShortDto>>();
        
        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IFinanceSourcesRepository repository) =>
            {
                var fs = await repository.GetByIdAsync(id);
                return fs is not null ? Results.Json(fs) : Results.NotFound();
            })
            .WithTags(Tag)
            .WithName("Get Finance source by Id")
            .RequireAuthorization()
            .Produces<FinanceSourceDto>()
            .Produces(StatusCodes.Status404NotFound);
        
        
        endpoints.MapPost(RoutePrefix, async ([FromBody]CreateFinanceSourceRequest request, [FromServices] IFinanceSourcesRepository repository) =>
            {
                var id = await repository.CreateAsync(new CreateFinanceSourceDto(request.Number, request.Name, request.FundType,
                    request.Start, request.End, request.Kpkvk, request.TotalEquipment, request.TotalMaterials, request.TotalServices));
                var fs = await repository.GetByIdAsync(id);

                return Results.Json(fs, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateFinanceSourceRequest>()
            .WithTags(Tag)
            .WithName("Create Finance source")
            .RequireAuthorization()
            .Produces<FinanceSourceDto>(StatusCodes.Status201Created);

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateFinanceSourceRequest request, [FromRoute] int id, [FromServices] IFinanceSourcesRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.Id != id)
                    {
                        return PromasyResults.ValidationError(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new UpdateFinanceSourceDto(request.Id, request.Number, 
                        request.Name, request.FundType, request.Start, request.End, request.Kpkvk, 
                        request.TotalEquipment, request.TotalMaterials, request.TotalServices));

                    return Results.Ok(StatusCodes.Status202Accepted);
                })
            .WithValidator<UpdateFinanceSourceRequest>()
            .WithTags(Tag)
            .WithName("Update Finance source")
            .RequireAuthorization()
            .Produces(StatusCodes.Status202Accepted);

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async (int id, [FromServices] IFinanceSourcesRepository repository) =>
            {
                await repository.DeleteByIdAsync(id);
                return Results.Ok(StatusCodes.Status204NoContent);
            })
            .WithTags(Tag)
            .WithName("Delete Finance source by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ValidationErrorResponse>(StatusCodes.Status409Conflict);

        _fsSubModule.MapEndpoints(endpoints);
        
        return endpoints;
    }
}