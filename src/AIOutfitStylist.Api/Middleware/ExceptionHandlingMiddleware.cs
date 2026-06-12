using System.Net;
using System.Text.Json;

namespace AIOutfitStylist.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled API exception");
            context.Response.StatusCode = ex is InvalidOperationException ? (int)HttpStatusCode.BadRequest : (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                title = "Request failed",
                detail = ex.Message,
                status = context.Response.StatusCode,
                traceId = context.TraceIdentifier
            }));
        }
    }
}
