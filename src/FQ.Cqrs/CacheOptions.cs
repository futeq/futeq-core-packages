namespace FQ.Cqrs;

/// <summary>
/// Caching configuration for a query response.
/// </summary>
public sealed class CacheOptions
{
    /// <summary>
    /// Gets or sets the time-to-live for a cached entry. Defaults to 5 minutes.
    /// </summary>
    public TimeSpan Ttl { get; init; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the cache tags associated with an entry for targeted invalidation.
    /// </summary>
    public string[] Tags { get; init; } = [];
}