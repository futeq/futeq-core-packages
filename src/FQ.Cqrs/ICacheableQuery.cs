namespace FQ.Cqrs;

/// <summary>
/// Indicates that a query is cacheable.
/// </summary>
public interface ICacheableQuery
{
    /// <summary>
    /// Gets a stable cache key that uniquely identifies the query parameters.
    /// </summary>
    string CacheKey { get; }

    /// <summary>
    /// Gets cache options such as TTL and tags for the query result.
    /// </summary>
    CacheOptions CacheOptions { get; }
}