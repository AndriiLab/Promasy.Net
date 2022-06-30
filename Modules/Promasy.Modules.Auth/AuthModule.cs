using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Promasy.Modules.Auth.Interfaces;
using Promasy.Modules.Auth.Models;
using Promasy.Modules.Auth.Services;
using Promasy.Modules.Core.Auth;
using Promasy.Modules.Core.Modules;
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
        builder.AddAuthorization(options =>
        {
            options.AddPolicy("Role", policy => policy.Requirements.Add(new RoleAuthorizationRequirement()));
        });
        builder.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                };
            });

        return builder;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var app = (WebApplication) endpoints;
        app.UseAuthentication();
        app.UseAuthorization();

        endpoints.MapPost(RoutePrefix,
                async ([FromBody] UserCredentialsRequest request, IAuthService authService,
                    ITokenService jwtTokenService, HttpResponse response) =>
                {
                    var id = await authService.AuthAsync(request.User, request.Password);
                    if (id is null)
                    {
                        return PromasyResults.ValidationError("Incorrect username or password");
                    }
                    var tokens = await jwtTokenService.GenerateTokenAsync(id.Value);
                    SetRefreshTokenCookie(response, tokens.RefreshToken, tokens.RefreshExpiryTime);

                    return Results.Json(new TokenResponse(tokens.Token));
                })
            .WithValidator<UserCredentialsRequest>()
            .WithTags(Tag)
            .WithName("Login")
            .Produces<TokenResponse>();

        endpoints.MapGet($"{RoutePrefix}/refresh", async (ITokenService jwtTokenService, HttpRequest request, HttpResponse response) =>
            {
                var refreshToken = GetRefreshTokenFromCookie(request);
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Results.Unauthorized();
                }
                var tokens = await jwtTokenService.RefreshTokenAsync(refreshToken);
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

        endpoints.MapPost($"{RoutePrefix}/revoke", async ([FromBody] TokenRequest? tr,
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

public class RoleAuthorizationRequirement : IAuthorizationRequirement
{
}