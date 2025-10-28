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

    public AuthorizationBehavior(IEnumerable<IAuthorizer<TRequest>> authorizers)
        => _authorizers = authorizers ?? [];

    public async Task<Result> Handle(TRequest request,
        RequestHandlerDelegate<Result> next,
        CancellationToken ct)
    {
        foreach (var auth in _authorizers)
        {
            var decision = await auth.AuthorizeAsync(request, ct).ConfigureAwait(false);
            if (!decision.IsSuccess)
            {
                return decision;
            } 
        }

        return await next(ct).ConfigureAwait(false);
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

    public AuthorizationBehavior(IEnumerable<IAuthorizer<TRequest>> authorizers)
        => _authorizers = authorizers ?? [];

    public async Task<Result<TResponse>> Handle(TRequest request,
        RequestHandlerDelegate<Result<TResponse>> next,
        CancellationToken ct)
    {
        foreach (var auth in _authorizers)
        {
            var decision = await auth.AuthorizeAsync(request, ct).ConfigureAwait(false);
            if (!decision.IsSuccess)
                return Result<TResponse>.Fail(decision.Error!);
        }

        return await next(ct).ConfigureAwait(false);
    }
}