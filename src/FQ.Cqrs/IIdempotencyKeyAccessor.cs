namespace FQ.Cqrs;

/// <summary>
/// Provides an idempotency key (e.g., from an HTTP header).
/// </summary>
public interface IIdempotencyKeyAccessor
{
    /// <summary>
    /// Gets a stable idempotency key for the current request context, or <c>null</c> if none is available.
    /// </summary>
    /// <returns>The idempotency key value, if present.</returns>
    string? GetKey();
}