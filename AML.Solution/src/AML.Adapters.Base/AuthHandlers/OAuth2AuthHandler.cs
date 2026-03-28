using AML.Core.Entities;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AML.Adapters.Base.AuthHandlers;

public sealed class OAuth2AuthHandler(HttpClient httpClient)
{
    public async Task ApplyAsync(HttpRequestMessage message, ServiceAuth? auth, CancellationToken cancellationToken = default)
    {
        if (auth is null || auth.AuthType != Core.Enums.AuthType.OAuth2)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(auth.TokenUrl))
        {
            return;
        }

        var form = new Dictionary<string, string?>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = auth.EncryptedClientId,
            ["client_secret"] = auth.EncryptedClientSecret,
            ["scope"] = auth.Scope
        }
        .Where(x => !string.IsNullOrWhiteSpace(x.Value))
        .ToDictionary(x => x.Key, x => x.Value!);

        using var tokenRequest = new HttpRequestMessage(HttpMethod.Post, auth.TokenUrl)
        {
            Content = new FormUrlEncodedContent(form)
        };

        using var tokenResponse = await httpClient.SendAsync(tokenRequest, cancellationToken);
        tokenResponse.EnsureSuccessStatusCode();

        var tokenPayload = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);
        using var doc = JsonDocument.Parse(tokenPayload);
        var accessToken = doc.RootElement.TryGetProperty("access_token", out var tokenElement)
            ? tokenElement.GetString()
            : null;

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}
