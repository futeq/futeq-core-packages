namespace FQ.Cqrs;

/// <summary>
/// Cache facade used by the caching behavior.
/// </summary>
public interface IQueryCache
{
    /// <summary>
    /// Attempts to retrieve a cached value for the specified key.
    /// </summary>
    /// <typeparam name="T">The cached value type.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A tuple indicating whether the value was found and the value itself.</returns>
    Task<(bool Found, T? Value)> TryGetAsync<T>(string key, CancellationToken ct);

    /// <summary>
    /// Stores a value in the cache with the provided options.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to store.</param>
    /// <param name="options">Cache options including TTL and tags.</param>
    /// <param name="ct">A cancellation token.</param>
    Task SetAsync<T>(string key, T value, CacheOptions options, CancellationToken ct);

    /// <summary>
    /// Invalidates entries associated with the given tags.
    /// </summary>
    /// <param name="tags">The tags to invalidate.</param>
    /// <param name="ct">A cancellation token.</param>
    Task InvalidateByTagsAsync(IEnumerable<string> tags, CancellationToken ct);
}