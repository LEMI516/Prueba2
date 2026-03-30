namespace AML.Gateway.Middleware;

public sealed class TenantResolutionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue("X-Client-Code", out var clientCode))
        {
            context.Items["ClientCode"] = clientCode.ToString();
        }

        await next(context);
    }
}
