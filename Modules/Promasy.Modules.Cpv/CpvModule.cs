using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Promasy.Modules.Core.Exceptions;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.OpenApi;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Cpv.Interfaces;
using Promasy.Modules.Cpv.Models;

namespace Promasy.Modules.Cpv;

public class CpvModule : IModule
{
    public string Tag { get; } = "CPV";
    public string RoutePrefix { get; } = "/api/cpv";

    public IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(RoutePrefix, async ([AsParameters] GetCpvsRequest request, [FromServices] ICpvsRepository repository) =>
            {
                var result = await repository.GetCpvsAsync(request);
                return TypedResults.Ok(result);
            })
            .WithValidator<GetCpvsRequest>()
            .WithApiDescription(Tag, "GetCpvList", "Get CPVs list")
            .RequireAuthorization();

        endpoints.MapGet($"{RoutePrefix}/{{code}}",
                async ([AsParameters] GetCpvByCodeRequest request, [FromServices] ICpvsRepository repository) =>
                {
                    var result = await repository.GetCpvByCodeAsync(request.Code);
                    if (result is null)
                    {
                        throw new ApiException(null, StatusCodes.Status404NotFound);
                    }

                    return TypedResults.Ok(result);
                })
            .WithValidator<GetCpvByCodeRequest>()
            .WithApiDescription(Tag, "GetCpvByCode", "Get CPV by code")
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        return endpoints;
    }
}