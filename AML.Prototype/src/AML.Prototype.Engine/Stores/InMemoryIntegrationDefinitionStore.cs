using AML.Prototype.Contracts.Models;
using AML.Prototype.Engine.Abstractions;
using System.Collections.Concurrent;

namespace AML.Prototype.Engine.Stores;

public sealed class InMemoryIntegrationDefinitionStore : IIntegrationDefinitionStore
{
    private readonly ConcurrentDictionary<string, IntegrationDefinition> _definitions =
        new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<IntegrationDefinition> GetAll()
    {
        return _definitions.Values
            .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public IntegrationDefinition? GetByKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        return _definitions.TryGetValue(key, out var definition) ? definition : null;
    }

    public IntegrationDefinition Upsert(string key, UpsertIntegrationDefinitionRequest request)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(request);

        var normalizedKey = key.Trim();
        return _definitions.AddOrUpdate(
            normalizedKey,
            _ => CreateDefinition(normalizedKey, request),
            (_, existing) => UpdateDefinition(existing, request));
    }

    public bool Delete(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return false;
        }

        return _definitions.TryRemove(key.Trim(), out _);
    }

    private static IntegrationDefinition CreateDefinition(
        string key,
        UpsertIntegrationDefinitionRequest request)
    {
        Validate(request);
        return new IntegrationDefinition
        {
            Key = key,
            Name = request.Name.Trim(),
            BaseUrl = request.BaseUrl.TrimEnd('/'),
            Path = NormalizePath(request.Path),
            Method = request.Method,
            TimeoutSeconds = request.TimeoutSeconds,
            DefaultHeaders = new Dictionary<string, string>(
                request.DefaultHeaders ?? new Dictionary<string, string>(),
                StringComparer.OrdinalIgnoreCase),
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    private static IntegrationDefinition UpdateDefinition(
        IntegrationDefinition existing,
        UpsertIntegrationDefinitionRequest request)
    {
        Validate(request);
        existing.Name = request.Name.Trim();
        existing.BaseUrl = request.BaseUrl.TrimEnd('/');
        existing.Path = NormalizePath(request.Path);
        existing.Method = request.Method;
        existing.TimeoutSeconds = request.TimeoutSeconds;
        existing.DefaultHeaders = new Dictionary<string, string>(
            request.DefaultHeaders ?? new Dictionary<string, string>(),
            StringComparer.OrdinalIgnoreCase);
        existing.UpdatedAtUtc = DateTime.UtcNow;
        return existing;
    }

    private static void Validate(UpsertIntegrationDefinitionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("El nombre de la integración es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.BaseUrl))
        {
            throw new ArgumentException("La base URL es obligatoria.");
        }

        if (!Uri.TryCreate(request.BaseUrl, UriKind.Absolute, out _))
        {
            throw new ArgumentException("La base URL no es válida.");
        }

        if (request.TimeoutSeconds is <= 0 or > 300)
        {
            throw new ArgumentException("TimeoutSeconds debe estar entre 1 y 300.");
        }
    }

    private static string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return "/";
        }

        return path.StartsWith('/') ? path : $"/{path}";
    }
}
