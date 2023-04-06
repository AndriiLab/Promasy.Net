using Microsoft.AspNetCore.Localization;
using Microsoft.OpenApi.Models;
using Promasy.Core.Constants;
using Promasy.Modules.Auth;
using Promasy.Modules.Core;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Cpv;
using Promasy.Modules.Employees;
using Promasy.Modules.Files;
using Promasy.Modules.Finances;
using Promasy.Modules.Manufacturers;
using Promasy.Modules.Orders;
using Promasy.Modules.Organizations;
using Promasy.Modules.Suppliers;
using Promasy.Modules.Units;
using Promasy.Persistence;
using Promasy.Web.App.ExceptionHandlers;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Configure logging
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console());

    builder.Services.AddPersistence(builder.Configuration);

    // Register application modules
    builder.Services.RegisterModule<CoreModule>(builder.Configuration);
    builder.Services.RegisterModule<AuthModule>(builder.Configuration);
    builder.Services.RegisterModule<CpvModule>(builder.Configuration);
    builder.Services.RegisterModule<FilesModule>(builder.Configuration);
    builder.Services.RegisterModule<UnitsModule>(builder.Configuration);
    builder.Services.RegisterModule<EmployeesModule>(builder.Configuration);
    builder.Services.RegisterModule<ManufacturersModule>(builder.Configuration);
    builder.Services.RegisterModule<SuppliersModule>(builder.Configuration);
    builder.Services.RegisterModule<OrganizationsModule>(builder.Configuration);
    builder.Services.RegisterModule<FinancesModule>(builder.Configuration);
    builder.Services.RegisterModule<OrdersModule>(builder.Configuration);

    // Localization
    builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddMvcCore()
        .AddDataAnnotationsLocalization();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(o =>
    {
        o.SwaggerDoc("V1", new OpenApiInfo
        {
            Version = "V1",
            Title = "WebAPI",
            Description = "Promasy WebAPI"
        });
        o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.",
        });
        o.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    builder.Services.AddCors();

    // Supported localization cultures
    builder.Services.Configure<RequestLocalizationOptions>(options =>
    {
        options.SetDefaultCulture(LocalizationCulture.EnglishUs)
            .AddSupportedCultures(LocalizationCulture.SupportedCultures)
            .AddSupportedUICultures(LocalizationCulture.SupportedCultures);
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    app.UseMiddleware<PromasyExceptionHandlerMiddleware>();

    // Map module endpoints
    app.MapEndpoints();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(o => { o.SwaggerEndpoint("/swagger/V1/swagger.json", "Promasy WebAPI"); });
    }

    app.UseHttpsRedirection();

    // Configure parse localization from request 
    app.UseRequestLocalization(new RequestLocalizationOptions()
        .SetDefaultCulture(LocalizationCulture.EnglishUs)
        .AddSupportedCultures(LocalizationCulture.SupportedCultures)
        .AddSupportedUICultures(LocalizationCulture.SupportedCultures)
        .AddInitialRequestCultureProvider(new CustomRequestCultureProvider(context =>
        {
            var requestLanguages = context.Request.Headers["Accept-Language"].ToString();
            var requestLanguage = requestLanguages.Split(',').FirstOrDefault() ?? string.Empty;
            string selectedLanguage;
            if (LocalizationCulture.SupportedCultures.Contains(requestLanguage))
            {
                selectedLanguage = requestLanguage;
                Log.Information("Request culture: {Culture}", requestLanguage);
            }
            else
            {
                selectedLanguage = LocalizationCulture.EnglishUs;
                Log.Information("Request culture not defined. Fallback to default culture: {Culture}", requestLanguage);
            }

            return Task.FromResult(new ProviderCultureResult(selectedLanguage, selectedLanguage))!;
        })));

    app.PreparePersistence();

    app.Run();

    Log.Information("Promasy API stopped cleanly");
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Promasy API terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}