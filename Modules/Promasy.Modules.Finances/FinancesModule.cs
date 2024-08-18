using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Core.Resources;
using Promasy.Domain.Employees;
using Promasy.Domain.Finances;
using Promasy.Modules.Core.Exceptions;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.OpenApi;
using Promasy.Modules.Core.Permissions;
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

    public WebApplication MapEndpoints(WebApplication app)
    {
        app.MapGet($"{RoutePrefix}/all-fund-types", ([FromServices] IStringLocalizer<FinanceFundType> localizer) =>
            {
                return TypedResults.Ok(Enum.GetValues<FinanceFundType>()
                    .Select(r => new SelectItem<int>((int) r, localizer[r.ToString()])));
            })
            .WithApiDescription(Tag, "GetAvailableFundTypes", "Get available fund types");
        
        app.MapGet(RoutePrefix, async ([AsParameters] FinanceSourcesPagedRequest request, [FromServices] IFinanceSourcesRepository repository) =>
            {
                var list = await repository.GetPagedListAsync(request);
                return TypedResults.Ok(list);
            })
            .WithAuthorizationAndValidation<FinanceSourcesPagedRequest>(app, Tag, "Get Finance sources list", PermissionTag.List,
                Enum.GetValues<RoleName>().Select(r => (r, PermissionCondition.None)).ToArray());
        
        app.MapGet($"{RoutePrefix}/{{id:int}}", async ([AsParameters] GetFinanceSourceRequest request, [FromServices] IFinanceSourcesRepository repository) =>
            {
                var fs = await repository.GetByIdAsync(request.Id);
                if (fs is null)
                {
                    throw new ApiException(null, StatusCodes.Status404NotFound);
                }
                return TypedResults.Ok(fs);
            })
            .Produces(StatusCodes.Status404NotFound)
            .WithAuthorizationAndValidation<GetFinanceSourceRequest>(app, Tag, "Get Finance source by Id", PermissionTag.Get,
        Enum.GetValues<RoleName>().Select(r =>
                (r, r switch
                    {
                        RoleName.User => PermissionCondition.SameSubDepartment,
                        RoleName.PersonallyLiableEmployee => PermissionCondition.SameDepartment,
                        RoleName.HeadOfDepartment => PermissionCondition.SameDepartment,
                        _ => PermissionCondition.None
                    })).ToArray());
        
        app.MapPost(RoutePrefix, async ([FromBody] CreateFinanceSourceRequest request, [FromServices] IFinanceSourcesRepository repository) =>
            {
                var id = await repository.CreateAsync(new CreateFinanceSourceDto(request.Number, request.Name, request.FundType,
                    request.Start, request.End, request.Kpkvk, request.TotalEquipment, request.TotalMaterials, request.TotalServices));
                var fs = await repository.GetByIdAsync(id);

                return TypedResults.Json(fs, statusCode: StatusCodes.Status201Created);
            })
            .WithValidator<CreateFinanceSourceRequest>()
            .WithAuthorization(app, Tag, "Create Finance source", PermissionTag.Create, RoleName.Administrator, RoleName.ChiefAccountant, RoleName.ChiefEconomist);

        app.MapPut($"{RoutePrefix}/{{id:int}}",
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
            .WithAuthorization(app, Tag, "Update Finance source", PermissionTag.Update, RoleName.Administrator, RoleName.ChiefAccountant, RoleName.ChiefEconomist);


        app.MapDelete($"{RoutePrefix}/{{id:int}}", async ([AsParameters] DeleteFinanceSourceRequest request, [FromServices] IFinanceSourcesRepository repository) =>
            {
                await repository.DeleteByIdAsync(request.Id);
                return TypedResults.NoContent();
            })
            .WithAuthorization(app, Tag, "Delete Finance source by Id", PermissionTag.Delete, RoleName.Administrator, RoleName.ChiefAccountant, RoleName.ChiefEconomist);

        _fsSubModule.MapEndpoints(app);
        
        return app;
    }
}