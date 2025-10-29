namespace FQ.Functions;

public sealed class IdempotencyOptions
{
    /// <summary>Header name to read. Default: "Idempotency-Key".</summary>
    public string HeaderName { get; init; } = "Idempotency-Key";
}