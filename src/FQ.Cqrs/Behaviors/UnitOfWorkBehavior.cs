using FQ.Results;
using MediatR;

namespace FQ.Cqrs.Behaviors;

/// <summary>
/// Wraps command handlers in a transactional scope using <see cref="INonTransactional"/>.
/// Queries and commands marked <see cref="INonTransactional"/> are not wrapped.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
public sealed class UnitOfWorkBehavior<TRequest> : IPipelineBehavior<TRequest, Result> where TRequest : notnull
{
    private readonly IUnitOfWork _uow;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkBehavior{TRequest}"/> class.
    /// </summary>
    /// <param name="uow">The unit of work instance.</param>
    public UnitOfWorkBehavior(IUnitOfWork uow) => _uow = uow;

    /// <inheritdoc/>
    public async Task<Result> Handle(TRequest request, RequestHandlerDelegate<Result> next, CancellationToken ct)
    {
        var isCommand = typeof(ICommand).IsAssignableFrom(typeof(TRequest))
                      || typeof(TRequest).GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));

        if (!isCommand || request is INonTransactional || _uow.HasActiveTransaction)
        {
            return await next(ct);
        }

        await _uow.BeginAsync(ct);
        
        try
        {
            var result = await next(ct);
            
            if (result.IsSuccess)
            {
                await _uow.CommitAsync(ct);
            }
            else
            {
                await _uow.RollbackAsync(ct);
            }
            
            return result;
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            throw;
        }
    }
}

/// <summary>
/// Wraps command handlers in a transactional scope using <see cref="IUnitOfWork"/>.
/// Queries and commands marked <see cref="INonTransactional"/> are not wrapped.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The successful response value type.</typeparam>
public sealed class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>> where TRequest : notnull
{
    private readonly IUnitOfWork _uow;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="uow">The unit of work instance.</param>
    public UnitOfWorkBehavior(IUnitOfWork uow) => _uow = uow;

    /// <inheritdoc/>
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken ct)
    {
        var isCommand = typeof(TRequest).GetInterfaces().Any(i => i == typeof(ICommand) || (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>)));

        if (!isCommand || request is INonTransactional || _uow.HasActiveTransaction)
        {
            return await next(ct);
        }

        await _uow.BeginAsync(ct);
        
        try
        {
            var result = await next(ct);
            
            if (result.IsSuccess)
            {
                await _uow.CommitAsync(ct);
            }
            else
            {
                await _uow.RollbackAsync(ct);
            }
            
            return result;
        }
        catch
        {
            await _uow.RollbackAsync(ct);
            
            throw;
        }
    }
}