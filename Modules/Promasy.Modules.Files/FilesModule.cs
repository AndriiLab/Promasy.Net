using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Modules.Core.Exceptions;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.OpenApi;
using Promasy.Modules.Core.Validation;
using Promasy.Modules.Files.Helpers;
using Promasy.Modules.Files.Models;
using Promasy.Modules.Files.Services;

namespace Promasy.Modules.Files;

public class FilesModule : IModule
{
    public string Tag { get; } = "Files";
    public string RoutePrefix { get; } = "/api/files";
    
    public IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        builder.AddTransient<IFileStorage, FileStorage>();
        return builder;
    }

    public WebApplication MapEndpoints(WebApplication app)
    {
        // todo: improve security
        app.MapGet($"{RoutePrefix}/{{fileName}}", async ([AsParameters] GetFileByNameRequest request,
                [FromServices] IFileStorage fs, [FromServices] IStringLocalizer<SharedResource> localizer) =>
            {
                var bytes = await fs.ReadFileAsync(request.FileName);
                if (!bytes.Any())
                {
                    throw new ApiException(localizer["File {0} not found", request.FileName],
                        StatusCodes.Status404NotFound);
                }

                return TypedResults.File(bytes, MediaTypeNameHelper.GetMediaTypeName(request.FileName));
            })
            .WithApiDescription(Tag, "GetFile", "Get file by name")
            .WithValidator<GetFileByNameRequest>()
            .Produces<FileResult>()
            .Produces(StatusCodes.Status404NotFound);


        return app;
    }
}