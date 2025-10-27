using FQ.Cqrs;
using Microsoft.AspNetCore.Http;

namespace FQ.AspNetCore;

/// <summary>
/// Reads the idempotency key from the HTTP header "Idempotency-Key".
/// </summary>
public sealed class HttpIdempotencyKeyAccessor : IIdempotencyKeyAccessor
{
    private readonly IHttpContextAccessor _ctx;

    /// <summary>Initializes the accessor.</summary>
    public HttpIdempotencyKeyAccessor(IHttpContextAccessor ctx) => _ctx = ctx;

    /// <inheritdoc />
    public string? GetKey()
        => _ctx.HttpContext?.Request.Headers.TryGetValue("Idempotency-Key", out var v) is true
            ? v.ToString()
            : null;
}