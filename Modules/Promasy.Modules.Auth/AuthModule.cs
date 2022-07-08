using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Promasy.Core.Resources;
using Promasy.Core.UserContext;
using Promasy.Modules.Auth.Interfaces;
using Promasy.Modules.Auth.Models;
using Promasy.Modules.Auth.Services;
using Promasy.Modules.Core.Auth;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.Policies;
using Promasy.Modules.Core.Responses;
using Promasy.Modules.Core.Validation;

namespace Promasy.Modules.Auth;

public class AuthModule : IModule
{
    public string Tag { get; } = "Auth";
    public string RoutePrefix { get; } = "/api/auth";

    public IServiceCollection RegisterServices(IServiceCollection builder, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("AuthToken");
        builder.Configure<TokenSettings>(jwtSection);

        var jwtSettings = jwtSection.Get<TokenSettings>();
        builder.AddAuthorization(o =>
        {
            o.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().Build();
            o.AddPolicy(AdminOnlyPolicy.Name, new AdminOnlyPolicy().GetPolicy()); // todo: add by interface
        });
        builder.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                };
            });
        
        builder.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        builder.AddTransient<IUserContext, UserContext.UserContext>();

        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var app = (WebApplication) endpoints;
        app.UseAuthentication();
        app.UseAuthorization();

        endpoints.MapPost(RoutePrefix,
                async ([FromBody] UserCredentialsRequest request, IAuthService authService,
                    ITokenService jwtTokenService, HttpResponse response, IStringLocalizer<SharedResource> localizer) =>
                {
                    var id = await authService.AuthAsync(request.User, request.Password);
                    if (id is null)
                    {
                        return PromasyResults.ValidationError(localizer["Incorrect username or password"]);
                    }
                    var tokens = await jwtTokenService.GenerateTokenAsync(id.Value);
                    SetRefreshTokenCookie(response, tokens.RefreshToken, tokens.RefreshExpiryTime);

                    return Results.Json(new TokenResponse(tokens.Token));
                })
            .WithValidator<UserCredentialsRequest>()
            .WithTags(Tag)
            .WithName("Login")
            .Produces<TokenResponse>();

        endpoints.MapGet($"{RoutePrefix}/refresh", async (ITokenService jwtTokenService, IAuthService authService, HttpRequest request, HttpResponse response) =>
            {
                var refreshToken = GetRefreshTokenFromCookie(request);
                var id = jwtTokenService.GetEmployeeIdFromRefreshToken(refreshToken);
                if (id is null)
                {
                    return Results.Unauthorized();
                }

                await authService.SetUserContextAsync(id.Value);
                var tokens = await jwtTokenService.RefreshTokenAsync(id.Value, refreshToken!);
                if (tokens is null)
                {
                    return Results.Unauthorized();
                }
                
                SetRefreshTokenCookie(response, tokens.RefreshToken, tokens.RefreshExpiryTime);

                return Results.Json(new TokenResponse(tokens.Token));
            })
            .WithTags(Tag)
            .WithName("Refresh Token")
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces<TokenResponse>();

        endpoints.MapPost($"{RoutePrefix}/revoke", async ([FromBody] RevokeTokenRequest? tr,
                ITokenService jwtTokenService, HttpRequest request) =>
            {
                var token = tr?.Token ?? GetRefreshTokenFromCookie(request);
                if (string.IsNullOrEmpty(token))
                {
                    return Results.NoContent();
                }

                await jwtTokenService.RevokeTokenAsync(token);

                return Results.NoContent();
            })
            .WithTags(Tag)
            .RequireAuthorization()
            .WithName("Revoke Token")
            .Produces(StatusCodes.Status204NoContent);

        return endpoints;
    }


    private const string RefreshTokenCookieKey = "refresh_token";
    private static void SetRefreshTokenCookie(HttpResponse response, string token, DateTime refreshExpiryTime)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = refreshExpiryTime
        };
        response.Cookies.Append(RefreshTokenCookieKey, token, cookieOptions);
    }

    private static string? GetRefreshTokenFromCookie(HttpRequest request)
    {
        return request.Cookies.TryGetValue(RefreshTokenCookieKey, out var rt) ? rt : null;
    }
}