namespace AML.SDK;

public sealed class AmlGatewayClientOptions
{
    public const string SectionName = "AmlGatewayClient";
    public string BaseUrl { get; set; } = string.Empty;
    public string IntegrationEndpoint { get; set; } = "api/integration/process-intent";
    public string? BearerToken { get; set; }
}
