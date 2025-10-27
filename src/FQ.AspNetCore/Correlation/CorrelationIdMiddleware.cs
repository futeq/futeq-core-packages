using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FQ.AspNetCore.Correlation;

/// <summary>
/// Middleware that reads or generates a correlation ID and exposes it via <see cref="ICorrelationContext"/>.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly CorrelationOptions _options;

    /// <summary>Create middleware.</summary>
    public CorrelationIdMiddleware(RequestDelegate next, IOptions<CorrelationOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    /// <summary>Execute middleware.</summary>
    public async Task Invoke(HttpContext ctx)
    {
        var header = _options.HeaderName;
        string? id = null;

        if (ctx.Request.Headers.TryGetValue(header, out var incoming) && !string.IsNullOrWhiteSpace(incoming))
            id = incoming.ToString();
        else if (_options.GenerateWhenMissing)
            id = Activity.Current?.Id ?? Guid.NewGuid().ToString("N");

        // attach to Items + DI
        if (id is not null)
        {
            ctx.Items[typeof(ICorrelationContext)] = new CorrelationContext(id);
            if (_options.WriteResponseHeader)
                ctx.Response.OnStarting(() =>
                {
                    ctx.Response.Headers[header] = id;
                    return Task.CompletedTask;
                });
        }

        // Make it available for DI resolutions during the request
        using var scope = ctx.RequestServices.CreateScope();
        await _next(ctx);
    }
}

internal static class CorrelationHttpContextExtensions
{
    public static string? GetCorrelationId(this HttpContext ctx, string headerName = "X-Correlation-Id")
    {
        if (ctx.Items.TryGetValue(typeof(ICorrelationContext), out var obj) && obj is ICorrelationContext cc)
            return cc.CorrelationId;
        if (ctx.Request.Headers.TryGetValue(headerName, out var v)) return v.ToString();
        return null;
    }
}