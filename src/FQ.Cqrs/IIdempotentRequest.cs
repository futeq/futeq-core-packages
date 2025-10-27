namespace FQ.Cqrs;

/// <summary>
/// Indicates a request should be processed idempotently.
/// </summary>
public interface IIdempotentRequest
{
    /// <summary>
    /// Gets the desired time-to-live for the idempotent response entry. If <c>null</c>, the default is used.
    /// </summary>
    TimeSpan? IdempotencyTtl { get; }
}