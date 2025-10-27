using Mapster;

namespace FQ.Mapping.Mapster;

/// <summary>
/// <see cref="IObjectMapper"/> implementation backed by Mapster.
/// </summary>
internal sealed class MapsterObjectMapper : IObjectMapper
{
    private readonly TypeAdapterConfig _config;

    /// <summary>
    /// Initializes a new <see cref="MapsterObjectMapper"/>.
    /// </summary>
    /// <param name="config">The Mapster type adapter configuration.</param>
    public MapsterObjectMapper(TypeAdapterConfig config)
        => _config = config ?? TypeAdapterConfig.GlobalSettings;

    /// <inheritdoc />
    public TDestination Map<TSource, TDestination>(TSource source)
        => source is null ? default! : source.Adapt<TDestination>(_config);

    /// <inheritdoc />
    public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source)
        => source.ProjectToType<TDestination>(_config);

    /// <inheritdoc />
    public IQueryable<TDestination> ProjectTo<TSource, TDestination>(IQueryable<TSource> source)
        => source.ProjectToType<TDestination>(_config);
}