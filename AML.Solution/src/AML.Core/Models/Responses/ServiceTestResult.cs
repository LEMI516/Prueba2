namespace AML.Core.Models.Responses;

public sealed class ServiceTestResult
{
    public bool Success { get; set; }
    public int? StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public long DurationMs { get; set; }
}
