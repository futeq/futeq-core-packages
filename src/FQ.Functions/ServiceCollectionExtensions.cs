using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FQ.Functions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFunctionsUtilities(
        this IServiceCollection services,
        Action<CorrelationOptions>? correlation = null,
        Action<IdempotencyOptions>? idempotency = null)
    {
        var c = new CorrelationOptions(); correlation?.Invoke(c);
        var i = new IdempotencyOptions(); idempotency?.Invoke(i);

        services.AddSingleton(c);
        services.AddSingleton(i);

        services.AddSingleton<FunctionExceptionMiddleware>();
        services.AddSingleton<CorrelationMiddleware>();
        services.AddSingleton<IdempotencyMiddleware>();

        services.AddScoped<HttpIdempotencyKeyAccessor>();
        services.AddScoped<ICorrelationContext>(_ => new CorrelationContext(null));
        
        return services;
    }
}