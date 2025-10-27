namespace FQ.Mapping;

/// <summary>
/// Utilities for creating fallback in-memory projections when an adapter cannot provide expression trees.
/// </summary>
public static class ProjectionHelpers
{
    /// <summary>
    /// Builds a naive projection by materializing to memory and invoking <see cref="IObjectMapper.Map{TSource,TDestination}(TSource)"/>.
    /// </summary>
    public static IQueryable<TDestination> ProjectToInMemory<TDestination>(
        IQueryable source,
        IObjectMapper mapper)
        => source.Cast<object>()
            .AsEnumerable()
            .Select(x => mapper.Map<object, TDestination>(x))
            .AsQueryable();

    /// <summary>
    /// Builds a naive projection for a strongly-typed source sequence.
    /// </summary>
    public static IQueryable<TDestination> ProjectToInMemory<TSource, TDestination>(
        IQueryable<TSource> source,
        IObjectMapper mapper)
        => source.AsEnumerable()
            .Select(x => mapper.Map<TSource, TDestination>(x))
            .AsQueryable();
}