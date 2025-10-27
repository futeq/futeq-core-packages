using FQ.Results;

namespace FQ.Mapping;

/// <summary>
/// Extensions for mapping <see cref="Result"/> values using an <see cref="IObjectMapper"/>.
/// Keeps Application layer free of direct mapping-library calls.
/// </summary>
public static class ResultMappingExtensions
{
    /// <summary>
    /// Maps the successful value of a <see cref="Result{T}"/> to <typeparamref name="TDestination"/>, preserving failures.
    /// </summary>
    public static Result<TDestination> MapResult<TSource, TDestination>(
        this IObjectMapper mapper,
        Result<TSource> result)
        => result.IsSuccess
            ? Result<TDestination>.Ok(mapper.Map<TSource, TDestination>(result.Value!))
            : Result<TDestination>.Fail(result.Error!);

    /// <summary>
    /// Asynchronously maps the successful value of a <see cref="Result{T}"/> to <typeparamref name="TDestination"/>, preserving failures.
    /// </summary>
    public static async Task<Result<TDestination>> MapResultAsync<TSource, TDestination>(
        this IObjectMapper mapper,
        Task<Result<TSource>> resultTask,
        CancellationToken ct = default)
    {
        var r = await resultTask.ConfigureAwait(false);
        if (!r.IsSuccess) return Result<TDestination>.Fail(r.Error!);
        var mapped = await mapper.MapAsync<TSource, TDestination>(r.Value!, ct).ConfigureAwait(false);
        return Result<TDestination>.Ok(mapped);
    }
}