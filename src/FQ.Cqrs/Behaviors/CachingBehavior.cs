using System.Diagnostics.CodeAnalysis;
using FQ.Results;
using MediatR;

namespace FQ.Cqrs.Behaviors;

/// <summary>
/// Caches successful responses for queries that implement <see cref="ICacheableQuery"/>.
/// Never runs for command pipelines (i.e., handlers returning non-generic <see cref="Result"/>).
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The successful value type for <see cref="Result{T}"/>.</typeparam>
public sealed class CachingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, Result<TResponse>>
    where TRequest : notnull
{
    private readonly IQueryCache _cache;

    public CachingBehavior(IQueryCache cache) => _cache = cache;

    public async Task<Result<TResponse>> Handle(
        TRequest request,
        RequestHandlerDelegate<Result<TResponse>> next,
        CancellationToken ct)
    {
        // 1) Never cache command pipelines (those that effectively close as Result<Result>)
        if (typeof(TResponse) == typeof(Result))
            return await next(ct);

        // 2) Only cache explicit, cacheable queries
        if (request is not ICacheableQuery cacheable)
            return await next(ct);

        // Defensive key check
        if (string.IsNullOrWhiteSpace(cacheable.CacheKey))
            return await next(ct);

        // 3) Try cache
        var (found, value) = await _cache.TryGetAsync<TResponse>(cacheable.CacheKey, ct).ConfigureAwait(false);
        if (found && TryUnwrap(value, out var unwrapped))
            return Result<TResponse>.Ok(unwrapped);

        // 4) Execute handler
        var result = await next(ct).ConfigureAwait(false);

        // 5) Cache only successful results
        if (result.IsSuccess)
        {
            await _cache.SetAsync(cacheable.CacheKey, result.Value!, cacheable.CacheOptions, ct)
                        .ConfigureAwait(false);
        }

        return result;
    }

    // Helps generic type-flow; ensures null/defaults aren’t cached/restored accidentally
    private static bool TryUnwrap([AllowNull] TResponse candidate, out TResponse value)
    {
        value = candidate!;
        // If TResponse is a reference type: accept non-null
        if (!typeof(TResponse).IsValueType)
            return candidate is not null;

        // If TResponse is a value type: accept all (default(T) allowed) — adjust if you want to skip default structs
        return true;
    }
}