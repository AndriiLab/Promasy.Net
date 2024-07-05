using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Promasy.Application.Interfaces;
using Promasy.Core.Resources;
using Promasy.Modules.Auth.Helpers;
using Promasy.Modules.Auth.Interfaces;
using Promasy.Modules.Auth.Models;
using Promasy.Modules.Auth.Services;
using Promasy.Modules.Core.Exceptions;
using Promasy.Modules.Core.Modules;
using Promasy.Modules.Core.OpenApi;
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
        ArgumentNullException.ThrowIfNull(jwtSettings);
        builder.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().Build());
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

    public WebApplication MapEndpoints(WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapPost(RoutePrefix,
                async ([FromBody] UserCredentialsRequest request, IAuthService authService,
                    ITokenService jwtTokenService, HttpResponse response, IStringLocalizer<SharedResource> localizer) =>
                {
                    var id = await authService.AuthAsync(request.User, request.Password);
                    if (id is null)
                    {
                        throw new ApiException(localizer["Incorrect username or password"]);
                    }
                    var tokens = await jwtTokenService.GenerateTokenAsync(id.Value);
                    RefreshTokenCookieHelper.SetCookie(response, tokens.RefreshToken, tokens.RefreshExpiryTime);

                    return TypedResults.Ok(new TokenResponse(tokens.Token));
                })
            .WithValidator<UserCredentialsRequest>()
            .WithApiDescription(Tag, "Login", "Generate Token");

        app.MapGet($"{RoutePrefix}/refresh", async (ITokenService jwtTokenService, IAuthService authService, HttpRequest request, HttpResponse response) =>
            {
                var refreshToken = RefreshTokenCookieHelper.GetFromCookie(request);
                var id = jwtTokenService.GetEmployeeIdFromRefreshToken(refreshToken);
                if (id is null)
                {
                    throw new ApiException(null, StatusCodes.Status401Unauthorized);
                }

                await authService.SetUserContextAsync(id.Value);
                var tokens = await jwtTokenService.RefreshTokenAsync(id.Value, refreshToken!);
                if (tokens is null)
                {
                    throw new ApiException(null, StatusCodes.Status401Unauthorized);
                }
                
                RefreshTokenCookieHelper.SetCookie(response, tokens.RefreshToken, tokens.RefreshExpiryTime);

                return TypedResults.Ok(new TokenResponse(tokens.Token));
            })
            .WithApiDescription(Tag, "RefreshToken", "Refresh Token")
            .Produces(StatusCodes.Status401Unauthorized);

        app.MapPost($"{RoutePrefix}/revoke", async ([FromBody] RevokeTokenRequest? tr,
                ITokenService jwtTokenService, HttpRequest request) =>
            {
                var token = tr?.Token ?? RefreshTokenCookieHelper.GetFromCookie(request);
                if (string.IsNullOrEmpty(token))
                {
                    return TypedResults.NoContent();
                }

                await jwtTokenService.RevokeTokenAsync(token);

                return TypedResults.NoContent();
            })
            .RequireAuthorization()
            .WithApiDescription(Tag, "RevokeToken", "Revoke Token");

        return app;
    }

}