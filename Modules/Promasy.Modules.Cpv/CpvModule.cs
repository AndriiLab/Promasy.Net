using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Cpv.Dtos;
using Promasy.Modules.Cpv.Interfaces;
using Promasy.Modules.Cpv.Models;

namespace Promasy.Modules.Cpv;

public class CpvModule : IModule
{
    public const string Tag = "CPV";
    public const string RoutePrefix = "/api/cpv";

    public IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(RoutePrefix, async (GetCpvsRequest request, [FromServices] ICpvsRepository repository) =>
            {
                var result = await repository.GetCpvsAsync(request.Level, request.ParentId, request.Search);
                return Results.Json(result);
            })
            .WithValidator<GetCpvsRequest>()
            .WithTags(Tag)
            .WithName("Get CPVs list")
            .RequireAuthorization()
            .Produces<ICollection<CpvDto>>();

        endpoints.MapGet($"{RoutePrefix}/{{code}}",
                async (GetCpvByCodeRequest request, [FromServices] ICpvsRepository repository) =>
                {
                    var result = await repository.GetCpvByCodeAsync(request.Code);
                    return result is not null ? Results.Json(result) : Results.NotFound();
                })
            .WithValidator<GetCpvByCodeRequest>()
            .WithTags(Tag)
            .WithName("Get CPV by code")
            .RequireAuthorization()
            .Produces<CpvDto>()
            .Produces(StatusCodes.Status404NotFound);

        return endpoints;
    }
}