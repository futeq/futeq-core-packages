namespace FQ.Functions;

/// <summary>Options controlling correlation ID behavior in Azure Functions.</summary>
public sealed class CorrelationOptions
{
    public string HeaderName { get; init; } = "X-Correlation-Id";
    
    public bool GenerateWhenMissing { get; init; } = true;
}