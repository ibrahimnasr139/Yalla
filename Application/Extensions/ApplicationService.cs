namespace Application.Extensions
{
    public static class ApplicationService
    {
        public static void AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddAutoMapper(config => { }, typeof(ApplicationService).Assembly);

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ApplicationService).Assembly));

            builder.Services.AddValidatorsFromAssembly(typeof(ApplicationService).Assembly)
                .AddFluentValidationAutoValidation();
        }
    }
}