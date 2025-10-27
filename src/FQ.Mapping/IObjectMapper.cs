namespace FQ.Mapping;

/// <summary>
/// Minimal, transport-agnostic object mapping facade decoupled from any specific mapping library.
/// Provides object-to-object mapping and LINQ projection support for queryables.
/// </summary>
public interface IObjectMapper
{
    /// <summary>
    /// Maps a source instance to a destination type.
    /// </summary>
    /// <typeparam name="TSource">Source type.</typeparam>
    /// <typeparam name="TDestination">Destination type.</typeparam>
    /// <param name="source">The source instance.</param>
    /// <returns>A mapped <typeparamref name="TDestination"/> instance.</returns>
    TDestination Map<TSource, TDestination>(TSource source);

    /// <summary>
    /// Maps a source instance to a destination type asynchronously.
    /// Default implementation calls <see cref="Map{TSource,TDestination}(TSource)"/>.
    /// </summary>
    /// <typeparam name="TSource">Source type.</typeparam>
    /// <typeparam name="TDestination">Destination type.</typeparam>
    /// <param name="source">The source instance.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A mapped <typeparamref name="TDestination"/> instance.</returns>
    ValueTask<TDestination> MapAsync<TSource, TDestination>(TSource source, CancellationToken ct = default)
        => new(Map<TSource, TDestination>(source));

    /// <summary>
    /// Projects a queryable to a different element type using expression-based projection.
    /// </summary>
    /// <typeparam name="TDestination">Destination element type.</typeparam>
    /// <param name="source">The source queryable.</param>
    /// <returns>A projected queryable.</returns>
    IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source);

    /// <summary>
    /// Projects a queryable of <typeparamref name="TSource"/> elements to <typeparamref name="TDestination"/>.
    /// </summary>
    /// <typeparam name="TSource">Source element type.</typeparam>
    /// <typeparam name="TDestination">Destination element type.</typeparam>
    /// <param name="source">The source queryable.</param>
    /// <returns>A projected queryable.</returns>
    IQueryable<TDestination> ProjectTo<TSource, TDestination>(IQueryable<TSource> source);
}