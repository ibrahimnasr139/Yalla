using API.Middlewares;
using Application.Constants;
using Serilog;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace API.Extensions
{
    public static class PresentationService
    {
        public static void AddPresentationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

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

            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy("OtpByIp", httpContext =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 30,
                            Window = TimeSpan.FromMinutes(5),
                            SegmentsPerWindow = 5,
                            QueueLimit = 0,
                            AutoReplenishment = true
                        }
                    )
                );
            });
        }
    }
}