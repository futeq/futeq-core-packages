using FQ.Mapping.Mapster;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace FQ.Mapping;

/// <summary>
/// DI helpers for wiring Mapster as the <see cref="IObjectMapper"/> implementation.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Mapster and the default <see cref="IObjectMapper"/> implementation in the DI container.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configure">Optional configuration action for Mapster.</param>
    /// <returns>The same collection for chaining.</returns>
    public static IServiceCollection AddObjectMapper(
        this IServiceCollection services,
        Action<TypeAdapterConfig>? configure = null)
    {
        var cfg = new TypeAdapterConfig();
        configure?.Invoke(cfg);
        services.AddSingleton(cfg);
        services.AddSingleton<IObjectMapper, MapsterObjectMapper>();
        return services;
    }
}