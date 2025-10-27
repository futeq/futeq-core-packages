using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Options;

namespace FQ.Functions;

/// <summary>
/// Captures the Idempotency-Key header (async) and stores it in <see cref="FunctionContext.Items"/> for sync access later.
/// </summary>
public sealed class IdempotencyMiddleware : IFunctionsWorkerMiddleware
{
    private readonly IdempotencyOptions _options;

    public IdempotencyMiddleware(IOptions<IdempotencyOptions> options) => _options = options.Value;

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        var req = await context.GetHttpRequestDataAsync();
        if (req is not null &&
            req.Headers.TryGetValues(_options.HeaderName, out var values))
        {
            var key = values.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(key))
            {
                context.Items[IdempotencyContextKeys.IdempotencyKey] = key!;
            }
        }

        await next(context);
    }
}