using AML.Core.Entities;

namespace AML.Adapters.Base;

public sealed class FieldMapper
{
    public Dictionary<string, object?> MapRequest(
        IDictionary<string, object?> source,
        IEnumerable<ServiceFieldMapping> mappings)
    {
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var mapping in mappings)
        {
            if (source.TryGetValue(mapping.SourceField, out var value))
            {
                result[mapping.TargetField] = value;
                continue;
            }

            if (mapping.IsRequired)
            {
                result[mapping.TargetField] = mapping.DefaultValue;
            }
        }

        return result;
    }
}
