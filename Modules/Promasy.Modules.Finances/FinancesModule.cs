using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Domain.Finances;
using Promasy.Modules.Core.Exceptions;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.OpenApi;
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
                return TypedResults.Ok(Enum.GetValues<FinanceFundType>()
                    .Select(r => new SelectItem<int>((int) r, localizer[r.ToString()])));
            })
            .WithApiDescription(Tag, "GetAvailableFundTypes", "Get available fund types");
        
        endpoints.MapGet(RoutePrefix, async ([AsParameters] FinanceSourcesPagedRequest request, [FromServices] IFinanceSourcesRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithValidator<FinanceSourcesPagedRequest>()
            .WithApiDescription(Tag, "GetFinanceSourcesList", "Get Finance sources list")
            .RequireAuthorization();
        
        endpoints.MapGet($"{RoutePrefix}/{{id:int}}", async ([FromRoute] int id, [FromServices] IFinanceSourcesRepository repository) =>
            {
                var fs = await repository.GetByIdAsync(id);
                if (fs is null)
                {
                    throw new ApiException(null, StatusCodes.Status404NotFound);
                }
                return TypedResults.Ok(fs);
            })
            .WithApiDescription(Tag, "GetFinanceSourceById", "Get Finance source by Id")
            .RequireAuthorization()
            .Produces(StatusCodes.Status404NotFound);
        
        
        endpoints.MapPost(RoutePrefix, async ([FromBody] CreateFinanceSourceRequest request, [FromServices] IFinanceSourcesRepository repository) =>
            {
                var id = await repository.CreateAsync(new CreateFinanceSourceDto(request.Number, request.Name, request.FundType,
                    request.Start, request.End, request.Kpkvk, request.TotalEquipment, request.TotalMaterials, request.TotalServices));
                var fs = await repository.GetByIdAsync(id);

                return TypedResults.Json(fs, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateFinanceSourceRequest>()
            .WithApiDescription(Tag, "CreateFinanceSource", "Create Finance source")
            .RequireAuthorization();

        endpoints.MapPut($"{RoutePrefix}/{{id:int}}",
                async ([FromBody] UpdateFinanceSourceRequest request, [FromRoute] int id, [FromServices] IFinanceSourcesRepository repository,
            [FromServices] IStringLocalizer<SharedResource> localizer) =>
                {
                    if (request.Id != id)
                    {
                        throw new ApiException(localizer["Incorrect Id"]);
                    }

                    await repository.UpdateAsync(new UpdateFinanceSourceDto(request.Id, request.Number, 
                        request.Name, request.FundType, request.Start, request.End, request.Kpkvk, 
                        request.TotalEquipment, request.TotalMaterials, request.TotalServices));

                    return TypedResults.Accepted(string.Empty);
                })
            .WithValidator<UpdateFinanceSourceRequest>()
            .WithApiDescription(Tag, "UpdateFinanceSource", "Update Finance source")
            .RequireAuthorization();

        endpoints.MapDelete($"{RoutePrefix}/{{id:int}}", async ([FromQuery] int id, [FromServices] IFinanceSourcesRepository repository) =>
            {
                await repository.DeleteByIdAsync(id);
                return TypedResults.NoContent();
            })
            .WithApiDescription(Tag, "DeleteFinanceSourceById", "Delete Finance source by Id")
            .RequireAuthorization();

        _fsSubModule.MapEndpoints(endpoints);
        
        return endpoints;
    }
}