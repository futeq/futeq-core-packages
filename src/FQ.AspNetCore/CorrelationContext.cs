namespace FQ.AspNetCore;

/// <summary>Provides access to the current request's correlation ID.</summary>
public interface ICorrelationContext
{
    /// <summary>The current correlation identifier, if available.</summary>
    string? CorrelationId { get; }
}

internal sealed class CorrelationContext(string? id) : ICorrelationContext
{
    public string? CorrelationId { get; } = id;
}