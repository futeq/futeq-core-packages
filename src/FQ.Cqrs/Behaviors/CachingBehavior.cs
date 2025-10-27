using FQ.Results;
using MediatR;

namespace FQ.Cqrs.Behaviors;

/// <summary>
/// Caches responses for queries that implement <see cref="ICacheableQuery"/>.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The successful response value type.</typeparam>
public sealed class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>> where TRequest : notnull
{
    private readonly IQueryCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="cache">The query cache implementation.</param>
    public CachingBehavior(IQueryCache cache) => _cache = cache;

    /// <inheritdoc/>
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken ct)
    {
        if (request is not ICacheableQuery cacheable)
        {
            return await next(ct);
        }

        var (found, value) = await _cache.TryGetAsync<TResponse>(cacheable.CacheKey, ct);

        if (found)
        {
            return Result<TResponse>.Ok(value!);
        }

        var result = await next(ct);

        if (result.IsSuccess)
        {
            await _cache.SetAsync(cacheable.CacheKey, result.Value!, cacheable.CacheOptions, ct);
        }

        return result;
    }
}