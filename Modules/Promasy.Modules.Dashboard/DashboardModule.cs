using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Promasy.Domain.Orders;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.OpenApi;
using Promasy.Modules.Dashboard.Interfaces;

namespace Promasy.Modules.Dashboard;

public class DashboardModule : IModule
{
    public string Tag { get; } = "Dashboard";
    public string RoutePrefix { get; } = "/api/dashboard";

    public IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet($"{RoutePrefix}/orders-count", async ([FromQuery, BindRequired] int year, [FromServices] IDashboardRepository repository) =>
            {
                var result = await repository.GetOrdersCountAsync(year);
                return TypedResults.Ok(result);
            })
            .WithApiDescription(Tag, "GetOrdersCount", "Get Orders count")
            .RequireAuthorization();
        
        endpoints.MapGet($"{RoutePrefix}/funding-left", async ([FromQuery, BindRequired] int year, 
                [FromQuery, BindRequired] OrderType type, [FromServices] IDashboardRepository repository) =>
            {
                var result = await repository.GetFundingLeftAsync(type, year);
                return TypedResults.Ok(result);
            })
            .WithApiDescription(Tag, "GetFundingLeft", "Get funding left by type")
            .RequireAuthorization();
        
        return endpoints;
    }
}