using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Options;

namespace FQ.Functions;

/// <summary>
/// Captures the Idempotency-Key header (async) and stores it in <see cref="FunctionContext.Items"/> for sync access later.
/// </summary>
public sealed class IdempotencyMiddleware : IFunctionsWorkerMiddleware
{
    private readonly IdempotencyOptions _opt;
    private readonly FunctionRequestResolver _resolve;

    public IdempotencyMiddleware(IOptions<IdempotencyOptions> opt,
        FunctionRequestResolver? resolve = null)
    {
        _opt = opt.Value;
        _resolve = resolve ?? (async ctx => await ctx.GetHttpRequestDataAsync());
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var req = await _resolve(context);
        
        if (req is not null &&
            req.Headers.TryGetValues(_opt.HeaderName, out var vals))
        {
            var key = vals.FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(key))
            {
                context.Items[IdempotencyContextKeys.IdempotencyKey] = key!;
            }
        }
        await next(context);
    }
}