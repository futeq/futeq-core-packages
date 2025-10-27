using System.Text.Json;

namespace FQ.Cqrs.Behaviors;

/// <summary>
/// Options controlling idempotency behavior.
/// </summary>
public sealed class IdempotencyOptions
{
    /// <summary>
    /// Gets or sets the default time-to-live for idempotent entries. Defaults to 24 hours.
    /// </summary>
    public TimeSpan DefaultTtl { get; init; } = TimeSpan.FromHours(24);

    /// <summary>
    /// Gets or sets the serialized payload content type. Defaults to "application/json".
    /// </summary>
    public string? ContentType { get; init; } = "application/json";

    /// <summary>
    /// Gets or sets the serializer options used for payload storage.
    /// </summary>
    public JsonSerializerOptions JsonSerializerOptions { get; init; } = new(JsonSerializerDefaults.Web);
}