using System.Diagnostics;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Options;

namespace FQ.Functions;

/// <summary>Isolated worker middleware that propagates or generates a correlation ID.</summary>
public sealed class CorrelationMiddleware : IFunctionsWorkerMiddleware
{
    private readonly CorrelationOptions _opt;

    /// <summary>Create middleware.</summary>
    public CorrelationMiddleware(IOptions<CorrelationOptions> opt) => _opt = opt.Value;

    /// <inheritdoc/>
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        string? id = null;
        var req = await context.GetHttpRequestDataAsync();
        if (req is not null && req.Headers.TryGetValues(_opt.HeaderName, out var vals))
            id = vals.FirstOrDefault();

        id ??= Activity.Current?.Id ?? (_opt.GenerateWhenMissing ? Guid.NewGuid().ToString("N") : null);

        if (id is not null)
            context.Items[typeof(ICorrelationContext)] = new CorrelationContext(id);

        await next(context);
    }
}