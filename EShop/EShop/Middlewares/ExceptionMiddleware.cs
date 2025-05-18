using System.Diagnostics;
using System.Net.Mime;

namespace EShop.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            // Получаем traceId из текущей активности (если есть)
            var traceId =
                Activity.Current?.TraceId.ToString() ??
                context.TraceIdentifier; // Fallback на ASP.NET Core TraceId

            Activity.Current?.SetStatus(ActivityStatusCode.Error);
            Activity.Current?.AddException(exception);

            logger.LogError(exception, "Error: {Message}. TraceId: {TraceId}", exception.Message,
                traceId);

            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(new
            {
                error = "Something went wrong.. Contact the administrator",
                traceId
            });
        }
    }
}