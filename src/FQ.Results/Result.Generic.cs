using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace FQ.Results;

/// <summary>
/// Result type for operations producing a value, with value semantics and minimal allocations.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public readonly record struct Result<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the value when <see cref="IsSuccess"/> is <c>true</c>; otherwise <c>null</c>.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets the error when <see cref="IsSuccess"/> is <c>false</c>; otherwise <c>null</c>.
    /// </summary>
    public Error? Error { get; }

    private Result(bool isSuccess, T? value, Error? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result with the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value to carry.</param>
    /// <returns>A successful result carrying <paramref name="value"/>.</returns>
    public static Result<T> Ok(T value) => new(true, value, null);

    /// <summary>
    /// Creates a failed result with the specified <paramref name="error"/>.
    /// </summary>
    /// <param name="error">The error describing the failure.</param>
    /// <returns>A failed result.</returns>
    public static Result<T> Fail(Error error) => new(false, default, error ?? throw new ArgumentNullException(nameof(error)));

    /// <summary>
    /// Implicit conversion from <typeparamref name="T"/> to a successful <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    public static implicit operator Result<T>(T value) => Ok(value);

    /// <summary>
    /// Implicit conversion from <see cref="Error"/> to a failed <see cref="Result{T}"/>.
    /// </summary>
    /// <param name="error">The error describing the failure.</param>
    public static implicit operator Result<T>(Error error) => Fail(error);

    /// <summary>
    /// Projects a successful value into another value, preserving failures.
    /// </summary>
    /// <typeparam name="TOut">The projected value type.</typeparam>
    /// <param name="map">Projection function.</param>
    /// <returns>A projected successful result or the original failure.</returns>
    public Result<TOut> Map<TOut>(Func<T, TOut> map)
        => IsSuccess ? Result<TOut>.Ok(map(Value!)) : Result<TOut>.Fail(Error!);

    /// <summary>
    /// Asynchronously projects a successful value into another value, preserving failures.
    /// </summary>
    /// <typeparam name="TOut">The projected value type.</typeparam>
    /// <param name="map">Async projection function.</param>
    public async Task<Result<TOut>> MapAsync<TOut>(Func<T, Task<TOut>> map)
        => IsSuccess ? Result<TOut>.Ok(await map(Value!)) : Result<TOut>.Fail(Error!);

    /// <summary>
    /// Binds a successful value to another <see cref="Result{T}"/>, preserving failures.
    /// </summary>
    /// <typeparam name="TOut">The bound result value type.</typeparam>
    /// <param name="binder">Binding function.</param>
    public Result<TOut> Bind<TOut>(Func<T, Result<TOut>> binder)
        => IsSuccess ? binder(Value!) : Result<TOut>.Fail(Error!);

    /// <summary>
    /// Asynchronously binds a successful value to another <see cref="Result{T}"/>, preserving failures.
    /// </summary>
    /// <typeparam name="TOut">The bound result value type.</typeparam>
    /// <param name="binder">Async binding function.</param>
    public Task<Result<TOut>> BindAsync<TOut>(Func<T, Task<Result<TOut>>> binder)
        => IsSuccess ? binder(Value!) : Task.FromResult(Result<TOut>.Fail(Error!));

    /// <summary>
    /// Executes an action when the result is successful.
    /// </summary>
    /// <param name="action">Action to execute on the value.</param>
    /// <returns>The original result.</returns>
    public Result<T> Tap(Action<T> action)
    {
        if (IsSuccess)
        {
            action(Value!);
        }
        
        return this;
    }

    /// <summary>
    /// Checks a predicate on the successful value and converts to a failure if it does not hold.
    /// </summary>
    /// <param name="predicate">Predicate evaluated when successful.</param>
    /// <param name="errorFactory">Factory for the error using the current value.</param>
    /// <returns>The original result if the predicate holds; otherwise a failure from <paramref name="errorFactory"/>.</returns>
    public Result<T> Ensure(Func<T, bool> predicate, Func<T, Error> errorFactory)
        => IsSuccess && !predicate(Value!) ? Fail(errorFactory(Value!)) : this;

    /// <summary>
    /// Attempts to get the value when successful.
    /// </summary>
    /// <param name="value">The value if the result is successful; otherwise default.</param>
    /// <returns><c>true</c> when successful; otherwise <c>false</c>.</returns>
    public bool TryGetValue([MaybeNullWhen(false)] out T value)
    {
        value = IsSuccess ? Value! : default;
        
        return IsSuccess;
    }

    /// <summary>
    /// Converts this result to a <see cref="ProblemShape"/> suitable for API responses.
    /// </summary>
    /// <param name="instance">Optional URI pointing to the specific occurrence.</param>
    /// <returns>A problem shape describing the outcome.</returns>
    public ProblemShape ToProblemShape(string? instance = null)
        => IsSuccess
            ? new ProblemShape("OK", (int)HttpStatusCode.OK, "about:blank", "Success", instance)
            : Error!.ToProblemShape(instance);

    internal static Result<T> FromJsonResult(JsonResult<T> jsonResult) => new (jsonResult.IsSuccess, jsonResult.Value, jsonResult.Error);
    
    /// <inheritdoc />
    public override string ToString()
        => IsSuccess ? $"Result: Ok<{typeof(T).Name}>"
                     : $"Result: Fail<{typeof(T).Name}> ({Error!.Type}/{Error.Code})";
}