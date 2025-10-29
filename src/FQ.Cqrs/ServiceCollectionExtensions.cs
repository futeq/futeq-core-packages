using System.Reflection;
using FluentValidation;
using FQ.Cqrs.Behaviors;
using FQ.Cqrs.Caching;
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

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));   
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DomainEventsBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<>)); 
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

        services.AddSingleton<IQueryCache, NoOpQueryCache>();

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton(new IdempotencyOptions());
        services.AddSingleton(new PerformanceOptions());

        return services;
    }
}