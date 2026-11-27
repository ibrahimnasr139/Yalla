using Application.Contracts.Abstractions;
using Infrastructure.Seeders;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace Infrastructure.Extensions
{
    public static class InfrastructureService
    {
        public static void AddInfrastructureServices(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddDbContextPool<AppDbContext>(options =>
            {
                var connectionString = BuildPostgresConnectionString(configuration);
                var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                dataSourceBuilder.EnableDynamicJson();
                var dataSource = dataSourceBuilder.Build();
                options.UseNpgsql(dataSource);
            });

            builder.Services.AddHybridCache();
            var redisOptions = BuildRedisOptions(configuration);
            builder.Services.AddStackExchangeRedisCache(o => o.ConfigurationOptions = redisOptions);
            builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisOptions));

            builder.Services.AddHangfireServer();
            builder.Services.AddHangfire(config =>
            {
                var connectionString = BuildPostgresConnectionString(configuration);
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                      .UseSimpleAssemblyNameTypeSerializer()
                      .UseRecommendedSerializerSettings()
                      .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString));
            });

            builder.Services.AddHealthChecks()
                   .AddNpgSql(BuildPostgresConnectionString(configuration))
                   .AddHangfire(options => options.MinimumAvailableServers = 1)
                   .AddRedis(sp => sp.GetRequiredService<IConnectionMultiplexer>());

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

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

            builder.Services.AddOptions<JwtOptions>()
               .BindConfiguration(nameof(JwtOptions))
               .ValidateDataAnnotations()
               .ValidateOnStart();

            builder.Services.AddSingleton<ISmsService, FakeSmsService>();

            //builder.Services.AddOptions<WhySmsOptions>()
            //  .BindConfiguration(nameof(WhySmsOptions))
            //  .ValidateDataAnnotations()
            //  .ValidateOnStart();

            //builder.Services.AddHttpClient<ISmsService, WhySmsService>((sp, client) =>
            //{
            //    var option = sp.GetRequiredService<IOptions<WhySmsOptions>>().Value;
            //    client.BaseAddress = new Uri(option.BaseUrl);
            //    client.Timeout = TimeSpan.FromSeconds(15);
            //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", option.ApiToken);
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //});

            builder.Services.AddScoped<ISeeder, Seeder>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddSingleton<IOtpRateLimitService, OtpRateLimitService>();
            builder.Services.AddScoped<ITokenProvider, TokenProvider>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IPassengerRepository, PassengerRepository>();
            builder.Services.AddScoped<IDevicePushTokenRepository, DevicePushTokenRepository>();
        }
        private static string BuildPostgresConnectionString(IConfiguration configuration)
        {
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

            if (!string.IsNullOrWhiteSpace(databaseUrl))
            {
                var uri = new Uri(databaseUrl);
                var userInfo = uri.UserInfo.Split(':');

                return new NpgsqlConnectionStringBuilder
                {
                    Host = uri.Host,
                    Port = uri.Port,
                    Username = userInfo[0],
                    Password = userInfo[1],
                    Database = uri.AbsolutePath.Trim('/'),
                    SslMode = SslMode.Require
                }.ToString();
            }

            return configuration.GetConnectionString("DefaultConnection")!;
        }
        private static ConfigurationOptions BuildRedisOptions(IConfiguration configuration)
        {
            var url = Environment.GetEnvironmentVariable("REDIS_URL")
                ?? configuration.GetConnectionString("RedisConnection");

            if (string.IsNullOrWhiteSpace(url))
                throw new InvalidOperationException("Redis connection string is missing.");

            ConfigurationOptions options;
            if (url.StartsWith("redis://", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(url);
                options = new ConfigurationOptions
                {
                    Password = uri.UserInfo.Split(':').LastOrDefault(),
                    Ssl = false
                };
                options.EndPoints.Add(uri.Host, uri.Port);
            }
            else
            {
                options = ConfigurationOptions.Parse(url);
            }

            options.AbortOnConnectFail = false;
            return options;
        }
    }
}