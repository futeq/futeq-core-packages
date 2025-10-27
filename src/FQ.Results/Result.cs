using System.Net;

namespace FQ.Results;

/// <summary>
/// Result type for void-returning operations. Uses value semantics and avoids allocations on success.
/// </summary>
public readonly record struct Result
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the error when <see cref="IsSuccess"/> is <c>false</c>; otherwise <c>null</c>.
    /// </summary>
    public Error? Error { get; }

    private Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Ok() => new(true, null);

    /// <summary>
    /// Creates a failed result with the specified <paramref name="error"/>.
    /// </summary>
    /// <param name="error">The error describing the failure.</param>
    public static Result Fail(Error error) => new(false, error ?? throw new ArgumentNullException(nameof(error)));

    /// <summary>
    /// Implicit conversion from <see cref="Error"/> to a failed <see cref="Result"/>.
    /// </summary>
    /// <param name="error">The error describing the failure.</param>
    public static implicit operator Result(Error error) => Fail(error);

    /// <summary>
    /// Ensures a post-condition holds on a successful result.
    /// </summary>
    /// <param name="condition">A predicate evaluated only when the result is successful.</param>
    /// <param name="errorFactory">Factory for the error returned when the condition fails.</param>
    /// <returns>
    /// The original result if successful and the condition holds; otherwise a failure produced by <paramref name="errorFactory"/>.
    /// </returns>
    public Result Ensure(Func<bool> condition, Func<Error> errorFactory)
        => IsSuccess && !condition() ? Fail(errorFactory()) : this;

    /// <summary>
    /// Executes an action when the result is successful.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <returns>The original result.</returns>
    public Result Tap(Action action)
    {
        if (IsSuccess) action();
        return this;
    }

    /// <summary>
    /// Converts this result to a <see cref="ProblemShape"/> suitable for API responses.
    /// </summary>
    /// <param name="instance">Optional URI pointing to the specific occurrence.</param>
    /// <returns>A problem shape describing the outcome.</returns>
    public ProblemShape ToProblemShape(string? instance = null)
        => Error is null
            ? new ProblemShape("OK", (int)HttpStatusCode.OK, "about:blank", "Success", instance)
            : Error.ToProblemShape(instance);

    /// <summary>
    /// Combines multiple results, returning the first failure encountered or <see cref="Ok"/>.
    /// </summary>
    /// <param name="results">Results to combine.</param>
    /// <returns>First failure or success if all succeeded.</returns>
    public static Result Combine(params Result[] results)
    {
        foreach (var r in results)
        {
            if (!r.IsSuccess)
            {
                return r;
            }
        }
        
        return Ok();
    }

    /// <inheritdoc />
    public override string ToString() => IsSuccess ? "Result: Ok" : $"Result: Fail ({Error!.Type}/{Error.Code})";
}