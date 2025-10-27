namespace FQ.Results;

/// <summary>
/// Convenience extensions for composing and lifting <see cref="Result"/> values,
/// including task-based variants for async flows.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Asynchronously projects a successful result's value to another type.
    /// </summary>
    /// <typeparam name="T">Input value type.</typeparam>
    /// <typeparam name="TOut">Projected value type.</typeparam>
    /// <param name="task">The source result task.</param>
    /// <param name="map">Projection function.</param>
    /// <returns>A projected successful result or the original failure.</returns>
    public static async Task<Result<TOut>> MapAsync<T, TOut>(this Task<Result<T>> task, Func<T, TOut> map)
    {
        var r = await task.ConfigureAwait(false);
        
        return r.IsSuccess ? Result<TOut>.Ok(map(r.Value!)) : Result<TOut>.Fail(r.Error!);
    }

    /// <summary>
    /// Asynchronously binds a successful result's value to another <see cref="Result{T}"/>.
    /// </summary>
    /// <typeparam name="T">Input value type.</typeparam>
    /// <typeparam name="TOut">Bound value type.</typeparam>
    /// <param name="task">The source result task.</param>
    /// <param name="binder">Async binding function.</param>
    /// <returns>A bound successful result or the original failure.</returns>
    public static async Task<Result<TOut>> BindAsync<T, TOut>(this Task<Result<T>> task, Func<T, Task<Result<TOut>>> binder)
    {
        var r = await task.ConfigureAwait(false);
        
        return r.IsSuccess ? await binder(r.Value!).ConfigureAwait(false) : Result<TOut>.Fail(r.Error!);
    }

    /// <summary>
    /// Creates a result from an optional value, failing with <paramref name="notFoundError"/> when <paramref name="value"/> is <c>null</c>.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <param name="value">The optional value.</param>
    /// <param name="notFoundError">Error to use when <paramref name="value"/> is <c>null</c>.</param>
    /// <returns>A successful or failed result.</returns>
    public static Result<T> FromNullable<T>(T? value, Error notFoundError)
        => value is null ? Result<T>.Fail(notFoundError) : Result<T>.Ok(value);

    /// <summary>
    /// Combines multiple non-generic results, returning the first failure encountered or success.
    /// </summary>
    /// <param name="results">Results to combine.</param>
    /// <returns>First failure or success.</returns>
    public static Result CombineAll(this IEnumerable<Result> results)
    {
        foreach (var r in results)
        {
            if (!r.IsSuccess) return r;
        }
        
        return Result.Ok();
    }

    /// <summary>
    /// Collects a sequence of <see cref="Result{T}"/> into a single <see cref="Result{T}"/> of array,
    /// short-circuiting on the first failure.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    /// <param name="results">Sequence of results.</param>
    /// <returns>Successful array of values or the first failure.</returns>
    public static Result<T[]> Collect<T>(this IEnumerable<Result<T>> results)
    {
        var list = new List<T>();
        
        foreach (var r in results)
        {
            if (!r.IsSuccess)
            {
                return Result<T[]>.Fail(r.Error!);
            }
            
            list.Add(r.Value!);
        }
        
        return Result<T[]>.Ok(list.ToArray());
    }
}