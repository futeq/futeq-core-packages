namespace FQ.Mapping;

/// <summary>
/// Marker interface indicating that the implementing type can be mapped to <typeparamref name="TDestination"/>.
/// </summary>
/// <typeparam name="TDestination">Destination type.</typeparam>
public interface IMapTo<TDestination> { }