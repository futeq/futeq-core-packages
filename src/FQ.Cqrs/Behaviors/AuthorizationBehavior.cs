using FQ.Results;
using MediatR;

namespace FQ.Cqrs.Behaviors;

/// <summary>
/// Executes zero or more <see cref="IAuthorizer{TRequest}"/> instances for the request and short-circuits on failure.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
public sealed class AuthorizationBehavior<TRequest> : IPipelineBehavior<TRequest, Result> where TRequest : notnull
{
    private readonly IEnumerable<IAuthorizer<TRequest>> _authorizers;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationBehavior{TRequest}"/> class.
    /// </summary>
    /// <param name="authorizers">The authorizers applicable to <typeparamref name="TRequest"/>.</param>
    public AuthorizationBehavior(IEnumerable<IAuthorizer<TRequest>> authorizers)
        => _authorizers = authorizers;

    /// <inheritdoc/>
    public async Task<Result> Handle(TRequest request, RequestHandlerDelegate<Result> next, CancellationToken ct)
    {
        foreach (var a in _authorizers)
        {
            var res = await a.AuthorizeAsync(request, ct);
            if (!res.IsSuccess) return res;
        }
        
        return await next(ct);
    }
}

/// <summary>
/// Executes zero or more <see cref="IAuthorizer{TRequest}"/> instances for the request and short-circuits on failure.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The successful response value type.</typeparam>
public sealed class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>> where TRequest : notnull
{
    private readonly IEnumerable<IAuthorizer<TRequest>> _authorizers;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorizationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="authorizers">The authorizers applicable to <typeparamref name="TRequest"/>.</param>
    public AuthorizationBehavior(IEnumerable<IAuthorizer<TRequest>> authorizers)
        => _authorizers = authorizers;

    /// <inheritdoc/>
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken ct)
    {
        foreach (var a in _authorizers)
        {
            var res = await a.AuthorizeAsync(request, ct);
            if (!res.IsSuccess) return Result<TResponse>.Fail(res.Error!);
        }
        
        return await next(ct);
    }
}