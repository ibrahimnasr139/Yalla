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
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                var redisUrl = Environment.GetEnvironmentVariable("REDIS_URL")
                    ?? configuration.GetConnectionString("RedisConnection");

                if (redisUrl?.StartsWith("redis://") == true)
                {
                    var uri = new Uri(redisUrl);
                    var password = uri.UserInfo.Split(':').LastOrDefault();
                    redisUrl = $"{uri.Host}:{uri.Port},password={password},ssl=false";
                }
                options.Configuration = redisUrl;
            });

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
                   .AddRedis(Environment.GetEnvironmentVariable("REDIS_URL") ?? configuration.GetConnectionString("RedisConnection")!);

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<AppDbContext>()
               .AddDefaultTokenProviders();

            builder.Services.AddOptions<JwtOptions>()
               .BindConfiguration(nameof(JwtOptions))
               .ValidateDataAnnotations()
               .ValidateOnStart();

            builder.Services.AddScoped<ITokenProvider, TokenProvider>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        }
        public static string BuildPostgresConnectionString(IConfiguration configuration)
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
    }
}