namespace AML.Gateway.Middleware;

public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var started = DateTime.UtcNow;
        await next(context);
        var elapsed = (long)(DateTime.UtcNow - started).TotalMilliseconds;

        logger.LogInformation(
            "Request {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            elapsed);
    }
}
