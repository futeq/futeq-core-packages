namespace FQ.Cqrs.Caching;

internal sealed class NoOpQueryCache : IQueryCache
{
    public Task<(bool Found, T? Value)> TryGetAsync<T>(string key, CancellationToken ct)
        => Task.FromResult((false, default(T)));

    public Task SetAsync<T>(string key, T value, CacheOptions options, CancellationToken ct)
        => Task.CompletedTask;

    public Task InvalidateByTagsAsync(IEnumerable<string> tags, CancellationToken ct) => Task.CompletedTask;
}