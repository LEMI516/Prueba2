namespace AML.Gateway.Middleware;

public sealed class AuthenticationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Base placeholder: la validación JWT real se agrega en la siguiente iteración.
        await next(context);
    }
}
