namespace FQ.Mapping;

/// <summary>
/// Extensions for mapping collections via <see cref="IObjectMapper"/>.
/// </summary>
public static class EnumerableMappingExtensions
{
    /// <summary>
    /// Maps each element of a sequence into <typeparamref name="TDestination"/>.
    /// </summary>
    /// <typeparam name="TSource">Source element type.</typeparam>
    /// <typeparam name="TDestination">Destination element type.</typeparam>
    /// <param name="mapper">The mapper.</param>
    /// <param name="source">Source sequence.</param>
    /// <returns>Mapped sequence (deferred execution).</returns>
    public static IEnumerable<TDestination> MapEach<TSource, TDestination>(
        this IObjectMapper mapper,
        IEnumerable<TSource> source)
    {
        foreach (var item in source)
            yield return mapper.Map<TSource, TDestination>(item);
    }
}