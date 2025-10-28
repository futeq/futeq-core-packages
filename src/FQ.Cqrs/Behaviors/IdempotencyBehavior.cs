using System.Text;
using System.Text.Json;
using FQ.Results;
using MediatR;

namespace FQ.Cqrs.Behaviors;

/// <summary>
/// Applies idempotency semantics to commands implementing <see cref="IIdempotentRequest"/>.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
public sealed class IdempotencyBehavior<TRequest> : IPipelineBehavior<TRequest, Result> where TRequest : notnull
{
    private readonly IIdempotencyKeyAccessor _keyAccessor;
    private readonly IIdempotencyStore _store;
    private readonly IdempotencyOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdempotencyBehavior{TRequest}"/> class.
    /// </summary>
    /// <param name="keyAccessor">The accessor providing the idempotency key.</param>
    /// <param name="store">The persistent store for cached responses.</param>
    /// <param name="options">Behavior options.</param>
    public IdempotencyBehavior(IIdempotencyKeyAccessor keyAccessor, IIdempotencyStore store, IdempotencyOptions options)
    {
        _keyAccessor = keyAccessor;
        _store = store;
        _options = options;
    }

    /// <inheritdoc/>
    public async Task<Result> Handle(TRequest request, RequestHandlerDelegate<Result> next, CancellationToken ct)
    {
        if (request is not IIdempotentRequest idempotentRequest)
        {
            return await next(ct);
        }

        var key = _keyAccessor.GetKey();
        
        if (string.IsNullOrWhiteSpace(key))
        {
            return await next(ct);
        }

        var requestType = typeof(TRequest).FullName!;
        var found = await _store.TryGetAsync(key, requestType, ct);
        
        if (found.Found && found.Payload is not null)
        {
            return found.Payload.TryDeserializeResultFromJson(out var restored, _options.JsonSerializerOptions) 
                ? restored 
                : Result.Fail(Error.Conflict("idempotency_deser_failed", "Failed to deserialize cached result"));
        }

        var result = await next(ct);

        var ttl = idempotentRequest?.IdempotencyTtl ?? _options.DefaultTtl;
        var bytes = JsonSerializer.SerializeToUtf8Bytes(result, _options.JsonSerializerOptions);
        
        await _store.SetAsync(key, requestType, bytes, _options.ContentType, ttl, ct);

        return result;
    }
}

/// <summary>
/// Applies idempotency semantics to commands implementing <see cref="IIdempotentRequest"/>.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The successful response value type.</typeparam>
public sealed class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>> where TRequest : notnull
{
    private readonly IIdempotencyKeyAccessor _keyAccessor;
    private readonly IIdempotencyStore _store;
    private readonly IdempotencyOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdempotencyBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="keyAccessor">The accessor providing the idempotency key.</param>
    /// <param name="store">The persistent store for cached responses.</param>
    /// <param name="options">Behavior options.</param>
    public IdempotencyBehavior(IIdempotencyKeyAccessor keyAccessor, IIdempotencyStore store, IdempotencyOptions options)
    {
        _keyAccessor = keyAccessor;
        _store = store;
        _options = options;
    }

    /// <inheritdoc/>
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken ct)
    {
        if (request is not IIdempotentRequest)
        {
            return await next(ct);
        }

        var key = _keyAccessor.GetKey();
        
        if (string.IsNullOrWhiteSpace(key))
        {
            return await next(ct);
        }

        var requestType = typeof(TRequest).FullName!;
        var found = await _store.TryGetAsync(key, requestType, ct);
        
        if (found is { Found: true, Payload: not null })
        { 
            return found.Payload.TryDeserializeResultFromJson<TResponse>(out var restored, _options.JsonSerializerOptions) 
                ? restored! 
                : Result<TResponse>.Fail(Error.Conflict("idempotency_deser_failed", "Failed to deserialize cached result"));
        }

        var result = await next(ct);

        var ttl = (request as IIdempotentRequest)?.IdempotencyTtl ?? _options.DefaultTtl;
        var bytes = JsonSerializer.SerializeToUtf8Bytes(result, _options.JsonSerializerOptions);
        
        await _store.SetAsync(key, requestType, bytes, _options.ContentType, ttl, ct);

        return result;
    }
}