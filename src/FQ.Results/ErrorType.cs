namespace FQ.Results;

/// <summary>
/// Coarse error categories used for behavior and status mapping.
/// </summary>
public enum ErrorType
{
    /// <summary>Unspecified failure.</summary>
    Failure = 0,
    /// <summary>Input validation failed.</summary>
    Validation = 1,
    /// <summary>Requested resource was not found.</summary>
    NotFound = 2,
    /// <summary>Conflict with current resource state (e.g., duplicates).</summary>
    Conflict = 3,
    /// <summary>Authentication is required and has failed or not been provided.</summary>
    Unauthorized = 4,
    /// <summary>Authenticated but not permitted to perform the operation.</summary>
    Forbidden = 5,
    /// <summary>Precondition headers or concurrency requirements not met.</summary>
    PreconditionFailed = 6,
    /// <summary>Rate-limiting or quota exceeded.</summary>
    TooManyRequests = 7,
    /// <summary>Optimistic concurrency violation.</summary>
    Concurrency = 8,
    /// <summary>
    /// Request is invalid.
    /// </summary>
    BadRequest = 9,
    /// <summary>
    /// Request cannot be processed.
    /// </summary>
    Unprocessable = 10,
    /// <summary>
    /// Internal error has occured.
    /// </summary>
    Internal = 11,
    /// <summary>
    /// Upstream is not available.
    /// </summary>
    UpstreamUnavailable = 12,
    /// <summary>
    /// Bad gateway.
    /// </summary>
    BadGateway = 13,
    /// <summary>
    /// Gateway has timed out.
    /// </summary>
    GatewayTimeout = 14
}