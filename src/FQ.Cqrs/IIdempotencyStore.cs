namespace FQ.Cqrs;

/// <summary>
/// Persistent storage for idempotent request responses.
/// </summary>
public interface IIdempotencyStore
{
    /// <summary>
    /// Attempts to retrieve a previously stored response for a key and request type.
    /// </summary>
    /// <param name="key">The idempotency key.</param>
    /// <param name="requestType">The fully qualified request type name.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>
    /// A tuple indicating whether a value was found, along with the raw payload and its content type.
    /// </returns>
    Task<(bool Found, byte[]? Payload, string? ContentType)> TryGetAsync(string key, string requestType, CancellationToken ct);

    /// <summary>
    /// Stores a response payload for a key and request type with a time-to-live.
    /// </summary>
    /// <param name="key">The idempotency key.</param>
    /// <param name="requestType">The fully qualified request type name.</param>
    /// <param name="payload">The serialized response payload.</param>
    /// <param name="contentType">The payload content type (e.g., "application/json").</param>
    /// <param name="ttl">The duration to keep the entry.</param>
    /// <param name="ct">A cancellation token.</param>
    Task SetAsync(string key, string requestType, byte[] payload, string? contentType, TimeSpan ttl, CancellationToken ct);
}