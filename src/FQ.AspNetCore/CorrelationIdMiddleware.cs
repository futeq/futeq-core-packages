using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FQ.AspNetCore;

/// <summary>
/// Middleware that reads or generates a correlation ID and exposes it via <see cref="ICorrelationContext"/>.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private readonly CorrelationIdOptions _idOptions;

    /// <summary>Create middleware.</summary>
    public CorrelationIdMiddleware(RequestDelegate next, IOptions<CorrelationIdOptions> options)
    {
        _next = next;
        _idOptions = options.Value;
    }

    /// <summary>Execute middleware.</summary>
    public async Task Invoke(HttpContext ctx)
    {
        var header = _idOptions.HeaderName;
        string? id = null;

        if (ctx.Request.Headers.TryGetValue(header, out var incoming) && !string.IsNullOrWhiteSpace(incoming))
            id = incoming.ToString();
        else if (_idOptions.GenerateWhenMissing)
            id = Activity.Current?.Id ?? Guid.NewGuid().ToString("N");

        if (id is not null)
        {
            ctx.Items[typeof(ICorrelationContext)] = new CorrelationContext(id);

            if (_idOptions.WriteResponseHeader)
            {
                ctx.Response.Headers[header] = id;
                
                ctx.Response.OnStarting(static state =>
                {
                    var (http, name, value) = ((HttpContext http, string name, string value))state;
                    http.Response.Headers[name] = value;
                    return Task.CompletedTask;
                }, (ctx, header, id));
            }
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