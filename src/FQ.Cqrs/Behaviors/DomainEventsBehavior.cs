using FQ.Results;
using MediatR;

namespace FQ.Cqrs.Behaviors;

/// <summary>
/// After a successful handler (and commit, if a transaction is used), dispatches pending domain events.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
public sealed class DomainEventsBehavior<TRequest> : IPipelineBehavior<TRequest, Result> where TRequest : notnull
{
    private readonly IDomainEventDispatcher _dispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventsBehavior{TRequest}"/> class.
    /// </summary>
    /// <param name="dispatcher">The domain event dispatcher.</param>
    public DomainEventsBehavior(IDomainEventDispatcher dispatcher) => _dispatcher = dispatcher;

    /// <inheritdoc/>
    public async Task<Result> Handle(TRequest request, RequestHandlerDelegate<Result> next, CancellationToken ct)
    {
        var res = await next(ct);
        
        if (res.IsSuccess)
        {
            await _dispatcher.DispatchPendingAsync(ct);
        }

        return res;
    }
}

/// <summary>
/// After a successful handler (and commit, if a transaction is used), dispatches pending domain events.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The successful response value type.</typeparam>
public sealed class DomainEventsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>> where TRequest : notnull
{
    private readonly IDomainEventDispatcher _dispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventsBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="dispatcher">The domain event dispatcher.</param>
    public DomainEventsBehavior(IDomainEventDispatcher dispatcher) => _dispatcher = dispatcher;

    /// <inheritdoc/>
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken ct)
    {
        var res = await next(ct);
        if (res.IsSuccess)
            await _dispatcher.DispatchPendingAsync(ct);

        return res;
    }
}