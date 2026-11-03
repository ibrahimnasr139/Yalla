using API.Middlewares;
using Serilog;

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
        }
    }
}