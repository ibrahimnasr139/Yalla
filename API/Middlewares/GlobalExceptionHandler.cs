using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.Middlewares
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Something went wrong.",
                Status = StatusCodes.Status500InternalServerError,
            };
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(problemDetails);
            await httpContext.Response.WriteAsync(result, cancellationToken);
            return true;
        }
    }
}