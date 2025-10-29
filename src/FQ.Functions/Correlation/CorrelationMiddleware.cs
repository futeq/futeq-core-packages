using System.Diagnostics;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Options;

namespace FQ.Functions;

/// <summary>Isolated worker middleware that propagates or generates a correlation ID.</summary>
public sealed class CorrelationIdMiddleware : IFunctionsWorkerMiddleware
{
    private readonly CorrelationIdOptions _opt;
    private readonly FunctionRequestResolver _resolve;

    /// <summary>Create middleware.</summary>
    public CorrelationIdMiddleware(IOptions<CorrelationIdOptions> opt,
        FunctionRequestResolver? resolve = null)
    {
        _opt = opt.Value;
        _resolve = resolve ?? (async ctx => await ctx.GetHttpRequestDataAsync());
    }

    /// <inheritdoc/>
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        string? id = null;
        
        var req = await _resolve(context);
        
        if (req is not null && req.Headers.TryGetValues(_opt.HeaderName, out var vals))
        {
            id = vals.FirstOrDefault();
        }

        id ??= Activity.Current?.Id ?? (_opt.GenerateWhenMissing ? Guid.NewGuid().ToString("N") : null);

        if (id is not null)
        {
            context.Items[typeof(ICorrelationContext)] = new CorrelationContext(id);
        }

        await next(context);
    }
}