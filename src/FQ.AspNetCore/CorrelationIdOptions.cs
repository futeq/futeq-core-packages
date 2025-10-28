namespace FQ.AspNetCore;

/// <summary>Options controlling correlation ID behavior.</summary>
public sealed class CorrelationIdOptions
{
    /// <summary>Request header to read/write correlation ID. Default: "X-Correlation-Id".</summary>
    public string HeaderName { get; set; } = "X-Correlation-Id";

    /// <summary>When true, writes the correlation ID to the response header. Default: true.</summary>
    public bool WriteResponseHeader { get; set; } = true;

    /// <summary>
    /// When true and no ID is supplied, generate a new one. Default: true.
    /// Uses W3C traceparent/Activity if present, otherwise GUID in "N" format.
    /// </summary>
    public bool GenerateWhenMissing { get; set; } = true;
}