using Microsoft.OpenApi.Models;
using Promasy.Modules.Auth;
using Promasy.Modules.Core;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Cpv;
using Promasy.Modules.Employees;
using Promasy.Modules.Units;
using Promasy.Persistence;
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

    builder.Services.AddPersistence(builder.Configuration.GetConnectionString("DatabaseConnection"));

    // Register application modules
    builder.Services.RegisterModule<CoreModule>(builder.Configuration);
    builder.Services.RegisterModule<AuthModule>(builder.Configuration);
    builder.Services.RegisterModule<CpvModule>(builder.Configuration);
    builder.Services.RegisterModule<UnitsModule>(builder.Configuration);
    builder.Services.RegisterModule<EmployeesModule>(builder.Configuration);

    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddMvcCore();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(o =>
    {
        o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.",
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

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    
    app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

    // Map module endpoints
    app.MapEndpoints();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

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