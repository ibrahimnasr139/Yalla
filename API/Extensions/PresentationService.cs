using API.Middlewares;
using Application.Common.Options;
using Application.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace API.Extensions
{
    public static class PresentationService
    {
        public static void AddPresentationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>().AddProblemDetails();
            builder.Host.UseSerilog((context, services, configuration) =>
                           configuration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services));

            builder.Services.AddCors(options =>
            {
                var allowedOrigins = builder.Configuration.GetSection(AuthConstants.AllowedOrigins).Get<string[]>() ?? [];
                options.AddDefaultPolicy(policy =>
                {
                    policy.SetIsOriginAllowed(origin =>
                    {
                        if (!Uri.TryCreate(origin, UriKind.Absolute, out var requestUri))
                            return false;

                        return allowedOrigins.Any(allowedOrigin =>
                        {
                            if (!Uri.TryCreate(allowedOrigin, UriKind.Absolute, out var allowedUri))
                                return false;

                            var sameScheme = requestUri.Scheme.Equals(allowedUri.Scheme, StringComparison.OrdinalIgnoreCase);
                            var samePort = requestUri.Port == allowedUri.Port;
                            var sameDomain = requestUri.Host.Equals(allowedUri.Host, StringComparison.OrdinalIgnoreCase) ||
                                requestUri.Host.EndsWith($".{allowedUri.Host}", StringComparison.OrdinalIgnoreCase);

                            return sameScheme && samePort && sameDomain;
                        });
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
                });
            });

            var jwtSettings = builder.Configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecretKey!)),
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidAudience = jwtSettings?.Audience,
                };
            });

            builder.Services.AddAuthorization();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });

        }
    }
}