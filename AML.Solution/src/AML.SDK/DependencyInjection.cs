using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AML.SDK;

public static class DependencyInjection
{
    public static IServiceCollection AddAmlSdk(
        this IServiceCollection services,
        Action<AmlGatewayClientOptions> configure)
    {
        services.Configure(configure);
        services.AddHttpClient<IAmlGatewayClient, AmlGatewayClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<AmlGatewayClientOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            if (!string.IsNullOrWhiteSpace(options.BearerToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", options.BearerToken);
            }
        });

        return services;
    }
}
