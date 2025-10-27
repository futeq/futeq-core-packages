using FQ.Results;
using MediatR;

namespace FQ.Cqrs.Behaviors;

/// <summary>
/// After handler execution, invalidates cache tags when the request implements <see cref="ICacheInvalidation"/> and succeeds.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The successful response value type.</typeparam>
public sealed class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>> where TRequest : notnull
{
    private readonly IQueryCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheInvalidationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="cache">The query cache implementation.</param>
    public CacheInvalidationBehavior(IQueryCache cache) => _cache = cache;

    /// <inheritdoc/>
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken ct)
    {
        var res = await next(ct);

        if (res.IsSuccess && request is ICacheInvalidation inv && inv.InvalidateTags().Any())
            await _cache.InvalidateByTagsAsync(inv.InvalidateTags(), ct);

        return res;
    }
}

/// <summary>
/// After handler execution, invalidates cache tags when the request implements <see cref="ICacheInvalidation"/> and succeeds.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
public sealed class CacheInvalidationBehavior<TRequest> : IPipelineBehavior<TRequest, Result> where TRequest : notnull
{
    private readonly IQueryCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheInvalidationBehavior{TRequest}"/> class.
    /// </summary>
    /// <param name="cache">The query cache implementation.</param>
    public CacheInvalidationBehavior(IQueryCache cache) => _cache = cache;

    /// <inheritdoc/>
    public async Task<Result> Handle(TRequest request, RequestHandlerDelegate<Result> next, CancellationToken ct)
    {
        var res = await next(ct);

        if (res.IsSuccess && request is ICacheInvalidation inv && inv.InvalidateTags().Any())
        {
            await _cache.InvalidateByTagsAsync(inv.InvalidateTags(), ct);
        }

        return res;
    }
}