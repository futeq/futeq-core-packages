namespace FQ.Results;

/// <summary>
/// Represents a lightweight, serializable error that can be surfaced across layers
/// (Domain, Application, API) and mapped to HTTP problem details at the edge.
/// </summary>
/// <param name="Code">
/// A machine-readable error code (stable identifier, e.g. "order_not_found").
/// </param>
/// <param name="Message">
/// A human-readable description intended for logs or API responses.
/// </param>
/// <param name="Type">
/// A coarse category used for behavior and HTTP status mapping (e.g. <see cref="ErrorType.Validation"/>).
/// </param>
/// <param name="Target">
/// Optional target indicating the subject of the error (e.g. a property name for validation).
/// </param>
/// <param name="Metadata">
/// Optional metadata for additional fields (kept small; suitable for serialization).
/// </param>
public sealed record Error(
    string Code,
    string Message,
    ErrorType Type = ErrorType.Failure,
    string? Target = null,
    IReadOnlyDictionary<string, object>? Metadata = null)
{
    /// <summary>
    /// Creates a <see cref="ErrorType.BadRequest"/> error (HTTP 400).
    /// Use when the client sent malformed or invalid input.
    /// </summary>
    public static Error BadRequest(string code = "bad_request", string? message = null, string? target = null, IReadOnlyDictionary<string, object>? metadata = null)
        => new(code, message ?? "The request could not be understood or was missing required parameters.", ErrorType.BadRequest, target, metadata);
    
    /// <summary>
    /// Creates a <see cref="ErrorType.NotFound"/> error (HTTP 404).
    /// </summary>
    public static Error NotFound(string code = "not_found", string? message = null, string? target = null, IReadOnlyDictionary<string, object>? metadata = null)
        => new(code, message ?? "The requested resource was not found.", ErrorType.NotFound, target, metadata);

    /// <summary>
    /// Creates a <see cref="ErrorType.Conflict"/> error (HTTP 409).
    /// </summary>
    public static Error Conflict(string code = "conflict", string? message = null, string? target = null, IReadOnlyDictionary<string, object>? metadata = null)
        => new(code, message ?? "A conflict occurred with the current state of the resource.", ErrorType.Conflict, target, metadata);

    /// <summary>
    /// Creates an <see cref="ErrorType.Unprocessable"/> error (HTTP 422).
    /// Use when the request was syntactically valid but semantically invalid.
    /// </summary>
    public static Error Unprocessable(string code = "unprocessable", string? message = null, string? target = null, IReadOnlyDictionary<string, object>? metadata = null)
        => new(code, message ?? "The server understands the content type but cannot process the instructions.", ErrorType.Unprocessable, target, metadata);

    /// <summary>
    /// Creates an <see cref="ErrorType.Internal"/> error (HTTP 500).
    /// </summary>
    public static Error Internal(string code = "internal_error", string? message = null, string? target = null, IReadOnlyDictionary<string, object>? metadata = null)
        => new(code, message ?? "An unexpected error occurred on the server.", ErrorType.Internal, target, metadata);

    /// <summary>
    /// Creates an <see cref="ErrorType.UpstreamUnavailable"/> error (HTTP 503).
    /// </summary>
    public static Error UpstreamUnavailable(string provider, string? message = null, string? target = null, IReadOnlyDictionary<string, object>? metadata = null)
        => new($"{provider}_unavailable", message ?? $"{provider} is temporarily unavailable.", ErrorType.UpstreamUnavailable, target, metadata);

    /// <summary>
    /// Creates an <see cref="ErrorType.BadGateway"/> error (HTTP 502).
    /// </summary>
    public static Error BadGateway(string provider, string? message = null, string? target = null, IReadOnlyDictionary<string, object>? metadata = null)
        => new($"{provider}_bad_gateway", message ?? $"{provider} returned an invalid response.", ErrorType.BadGateway, target, metadata);

    /// <summary>
    /// Creates an <see cref="ErrorType.GatewayTimeout"/> error (HTTP 504).
    /// </summary>
    public static Error GatewayTimeout(string provider, string? message = null, string? target = null, IReadOnlyDictionary<string, object>? metadata = null)
        => new($"{provider}_timeout", message ?? $"{provider} did not respond in time.", ErrorType.GatewayTimeout, target, metadata);

    /// <summary>
    /// Creates a <see cref="Error"/> categorized as <see cref="ErrorType.Unauthorized"/>.
    /// </summary>
    /// <param name="code">Optional code (defaults to "unauthorized").</param>
    /// <param name="message">Optional message (defaults to "Unauthorized").</param>
    public static Error Unauthorized(string code = "unauthorized", string message = "Unauthorized")
        => new(code, message, ErrorType.Unauthorized);

    /// <summary>
    /// Creates a <see cref="Error"/> categorized as <see cref="ErrorType.Forbidden"/>.
    /// </summary>
    /// <param name="code">Optional code (defaults to "forbidden").</param>
    /// <param name="message">Optional message (defaults to "Forbidden").</param>
    public static Error Forbidden(string code = "forbidden", string message = "Forbidden")
        => new(code, message, ErrorType.Forbidden);

    /// <summary>
    /// Creates a <see cref="Error"/> categorized as <see cref="ErrorType.Validation"/>.
    /// </summary>
    /// <param name="code">Machine-readable code (e.g. "validation_failed").</param>
    /// <param name="message">Human-readable message (e.g. "Validation failed").</param>
    /// <param name="target">Optional field or property name that failed validation.</param>
    /// <param name="meta">
    /// Optional metadata; for field errors, store a dictionary { field =&gt; string[] messages } under key "errors".
    /// </param>
    public static Error Validation(string code, string message, string? target = null, IReadOnlyDictionary<string, object>? meta = null)
        => new(code, message, ErrorType.Validation, target, meta);

    /// <summary>
    /// Creates a <see cref="Error"/> categorized as <see cref="ErrorType.TooManyRequests"/>.
    /// </summary>
    /// <param name="code">Optional code (defaults to "too_many_requests").</param>
    /// <param name="message">Optional message (defaults to "Too many requests").</param>
    public static Error TooMany(string code = "too_many_requests", string message = "Too many requests")
        => new(code, message, ErrorType.TooManyRequests);

    /// <summary>
    /// Creates a failure <see cref="Error"/> from an <see cref="Exception"/>.
    /// </summary>
    /// <param name="ex">The originating exception.</param>
    /// <param name="code">Optional machine-readable code (defaults to "exception").</param>
    /// <param name="target">Optional target.</param>
    /// <param name="includeStack">When true, includes exception type and stack trace in <paramref name="Meta"/>.</param>
    public static Error FromException(Exception ex, string code = "exception", string? target = null, bool includeStack = false)
        => new(code, ex.Message, ErrorType.Failure, target,
            includeStack
                ? new Dictionary<string, object>
                {
                    [Constants.ExceptionKeyName] = ex.GetType().FullName!,
                    [Constants.StackTraceKeyName] = ex.StackTrace ?? string.Empty
                }
                : null);
}