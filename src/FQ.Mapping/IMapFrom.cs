namespace FQ.Mapping;

/// <summary>
/// Marker interface indicating that the implementing type can be created by mapping from <typeparamref name="TSource"/>.
/// Adapters (e.g., Mapster) can reflect over these to auto-register maps.
/// </summary>
/// <typeparam name="TSource">Source type.</typeparam>
public interface IMapFrom<TSource> { }