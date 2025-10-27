using FQ.Cqrs;
using Microsoft.Azure.Functions.Worker;

namespace FQ.Functions;

public sealed class HttpIdempotencyKeyAccessor : IIdempotencyKeyAccessor
{
    private readonly FunctionContext _context;

    public HttpIdempotencyKeyAccessor(FunctionContext context) => _context = context;

    /// <inheritdoc/>
    public string? GetKey()
        => _context.Items.TryGetValue(IdempotencyContextKeys.IdempotencyKey, out var v)
            ? v as string
            : null;
}