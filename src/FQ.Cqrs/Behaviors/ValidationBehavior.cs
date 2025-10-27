using FluentValidation;
using FQ.Results;
using MediatR;

namespace FQ.Cqrs.Behaviors;

/// <summary>
/// Validates requests using registered <see cref="IValidator{T}"/> instances and short-circuits on failure.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
public sealed class ValidationBehavior<TRequest> : IPipelineBehavior<TRequest, Result> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest}"/> class.
    /// </summary>
    /// <param name="validators">Validators applicable to <typeparamref name="TRequest"/>.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    /// <inheritdoc/>
    public async Task<Result> Handle(TRequest request, RequestHandlerDelegate<Result> next, CancellationToken ct)
    {
        if (!_validators.Any())
        {
            return await next(ct);
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = new List<(string Field, string Message, string? Code)>();

        foreach (var v in _validators)
        {
            var res = await v.ValidateAsync(context, ct);
            failures.AddRange(res.Errors
                .Where(f => f is not null)
                .Select(f => (f.PropertyName, f.ErrorMessage, f.ErrorCode))!);
        }

        if (failures.Count == 0)
        {
            return await next(ct);
        }

        return Result.Fail(Validation.FromFailures(failures));
    }
}

/// <summary>
/// Validates requests using registered <see cref="IValidator{T}"/> instances and short-circuits on failure.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The successful response value type.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>> where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validators">Validators applicable to <typeparamref name="TRequest"/>.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    /// <inheritdoc/>
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken ct)
    {
        if (!_validators.Any())
        {
            return await next(ct);
        }

        var context = new ValidationContext<TRequest>(request);
        var failures = new List<(string Field, string Message, string? Code)>();

        foreach (var v in _validators)
        {
            var res = await v.ValidateAsync(context, ct);
            
            failures.AddRange(res.Errors
                .Where(f => f is not null)
                .Select(f => (f.PropertyName, f.ErrorMessage, f.ErrorCode))!);
        }

        if (failures.Count == 0)
        {
            return await next(ct);
        }

        return Result<TResponse>.Fail(Validation.FromFailures(failures));
    }
}