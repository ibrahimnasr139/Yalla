using API.Extensions;
using Application.Extensions;
using Hangfire;
using Infrastructure.Extensions;
using Infrastructure.Seeders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddPresentationServices();
builder.AddApplicationServices();
builder.AddInfrastructureServices(builder.Configuration);
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ISeeder>();
    await seeder.SeedAsync();
}

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
    app.UseHangfireDashboard();
}
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors();
app.UseHealthChecks("/health");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();