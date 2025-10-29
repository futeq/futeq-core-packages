using FQ.Functions.Accessors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FQ.Functions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFunctionsUtilities(
        this IServiceCollection services,
        Action<CorrelationIdOptions>? correlation = null,
        Action<IdempotencyOptions>? idempotency = null)
    {
        var c = new CorrelationIdOptions(); correlation?.Invoke(c);
        var i = new IdempotencyOptions(); idempotency?.Invoke(i);

        services.AddSingleton(c);
        services.AddSingleton(i);

        services.AddSingleton<IFunctionContextAccessor, FunctionContextAccessor>();

        services.AddSingleton<FunctionExceptionMiddleware>();
        services.AddSingleton<CorrelationIdMiddleware>();
        services.AddSingleton<IdempotencyMiddleware>();
        services.AddSingleton<FunctionContextAccessorMiddleware>();

        services.AddScoped<HttpIdempotencyKeyAccessor>();
        services.AddScoped<ICorrelationContext>(_ => new CorrelationContext(null));
        
        return services;
    }
}