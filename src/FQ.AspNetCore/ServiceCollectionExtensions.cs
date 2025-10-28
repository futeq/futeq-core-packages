using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FQ.AspNetCore;

public static class ServiceCollectionExtensions
{
    /// <summary>Registers correlation + versioning utilities and ProblemDetails factory.</summary>
    public static IServiceCollection AddAspNetCoreUtilities(this IServiceCollection services, Action<CorrelationIdOptions>? correlation = null)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<ProblemDetailsFactory, ResultProblemDetailsFactory>();

        var c = new CorrelationIdOptions(); correlation?.Invoke(c);

        services.AddSingleton(c); 

        // Accessor for CQRS idempotency if desired by the app:
        services.AddScoped<HttpIdempotencyKeyAccessor>();
        services.AddScoped<ICorrelationContext>(sp =>
        {
            var http = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
            var id = http?.GetCorrelationId(c.HeaderName);
            return new CorrelationContext(id);
        });

        return services;
    }

    /// <summary>Registers the correlation + version header middlewares in the pipeline.</summary>
    public static IApplicationBuilder UseAspNetCoreUtilities(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>()
            .UseMiddleware<ExceptionHandlerMiddleware>();
    }
}