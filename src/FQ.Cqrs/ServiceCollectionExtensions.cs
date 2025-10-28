using System.Reflection;
using FluentValidation;
using FQ.Cqrs.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FQ.Cqrs;

/// <summary>
/// Dependency injection extensions for registering CQRS components.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers MediatR, FluentValidation, and Futeq pipeline behaviors in a recommended order.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="applicationAssembly">
    /// The assembly that contains your application layer (handlers, validators, etc.).
    /// </param>
    /// <returns>The same service collection for chaining.</returns>
    /// <remarks>
    /// Behavior order (outermost → innermost): Performance → Authorization → Validation → Idempotency → UnitOfWork → Handler → DomainEvents → CacheInvalidation → Caching.
    /// </remarks>
    public static IServiceCollection AddCqrsUtilities(this IServiceCollection services, Assembly applicationAssembly)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DomainEventsBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton(new IdempotencyOptions());
        services.AddSingleton(new PerformanceOptions());

        return services;
    }
}