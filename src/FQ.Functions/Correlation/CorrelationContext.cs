namespace FQ.Functions;

public interface ICorrelationContext
{
    /// <summary>The correlation identifier if available.</summary>
    string? CorrelationId { get; }
}

internal sealed class CorrelationContext(string? id) : ICorrelationContext
{
    public string? CorrelationId { get; } = id;
}