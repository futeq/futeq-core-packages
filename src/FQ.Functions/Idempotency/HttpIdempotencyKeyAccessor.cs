using FQ.Cqrs;
using Microsoft.Azure.Functions.Worker;

namespace FQ.Functions;

public sealed class HttpIdempotencyKeyAccessor : IIdempotencyKeyAccessor
{
    private readonly IFunctionContextAccessor _context;

    public HttpIdempotencyKeyAccessor(IFunctionContextAccessor context) => _context = context;

    /// <inheritdoc/>
    public string? GetKey()
    {
        var context = _context.Current;

        if (context is null)
        {
            return null;
        }

        if (context.Items.TryGetValue(IdempotencyContextKeys.IdempotencyKey, out var v) && v is string s)
        {
            return s;
        }

        return null;
    }
}