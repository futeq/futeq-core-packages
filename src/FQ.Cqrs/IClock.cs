namespace FQ.Cqrs;

/// <summary>
/// Provides the current UTC time.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Gets the current UTC timestamp.
    /// </summary>
    DateTime UtcNow { get; }
}