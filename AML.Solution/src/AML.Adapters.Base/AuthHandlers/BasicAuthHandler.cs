using System.Net.Http.Headers;
using System.Text;
using AML.Core.Entities;

namespace AML.Adapters.Base.AuthHandlers;

public sealed class BasicAuthHandler
{
    public void Apply(HttpRequestMessage request, ServiceAuth auth)
    {
        if (string.IsNullOrWhiteSpace(auth.EncryptedUsername) || string.IsNullOrWhiteSpace(auth.EncryptedPassword))
        {
            return;
        }

        // Placeholder: la desencriptación real se implementa en Infrastructure.Security.
        var username = auth.EncryptedUsername;
        var password = auth.EncryptedPassword;
        var basic = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);
    }
}
