using AML.Core.Entities;
using Microsoft.AspNetCore.WebUtilities;

namespace AML.Adapters.Base.AuthHandlers;

public static class ApiKeyAuthHandler
{
    public static HttpRequestMessage Apply(HttpRequestMessage request, ServiceAuth? auth)
    {
        if (auth is null || string.IsNullOrWhiteSpace(auth.ApiKeyHeaderName) || string.IsNullOrWhiteSpace(auth.EncryptedApiKey))
        {
            return request;
        }

        request.Headers.TryAddWithoutValidation(auth.ApiKeyHeaderName, auth.EncryptedApiKey);
        return request;
    }
}
