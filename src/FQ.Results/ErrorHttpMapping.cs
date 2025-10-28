using System.Net;

namespace FQ.Results;

/// <summary>
/// Helpers for mapping <see cref="Error"/> to HTTP semantics in a transport-agnostic way.
/// Consumers in the API layer can bridge this to ASP.NET's <c>ProblemDetails</c>.
/// </summary>
public static class ErrorHttpMapping
{
    /// <summary>
    /// Maps an <see cref="Error"/> to an <see cref="HttpStatusCode"/>.
    /// </summary>
    /// <param name="error">The error to map.</param>
    /// <returns>The corresponding HTTP status code.</returns>
    public static HttpStatusCode ToStatusCode(this Error error) => error.Type switch
    {
        ErrorType.Validation or ErrorType.BadRequest       => HttpStatusCode.BadRequest,           // 400
        ErrorType.Unauthorized                             => HttpStatusCode.Unauthorized,         // 401
        ErrorType.Forbidden                                => HttpStatusCode.Forbidden,            // 403
        ErrorType.NotFound                                 => HttpStatusCode.NotFound,             // 404
        ErrorType.Conflict                                 => HttpStatusCode.Conflict,             // 409
        ErrorType.TooManyRequests                          => (HttpStatusCode)429,
        ErrorType.Unprocessable                            => (HttpStatusCode)422,
        ErrorType.BadGateway                               => HttpStatusCode.BadGateway,           // 502
        ErrorType.UpstreamUnavailable                      => HttpStatusCode.ServiceUnavailable,   // 503
        ErrorType.GatewayTimeout                           => HttpStatusCode.GatewayTimeout,       // 504
        ErrorType.Internal or _                            => HttpStatusCode.InternalServerError,  // 500 default
    };

    /// <summary>
    /// Creates a <see cref="ProblemShape"/> from an <see cref="Error"/>.
    /// </summary>
    /// <param name="error">The error to convert.</param>
    /// <param name="instance">Optional URI pointing to the specific occurrence.</param>
    /// <returns>A <see cref="ProblemShape"/> instance.</returns>
    public static ProblemShape ToProblemShape(this Error error, string? instance = null)
    {
        var status = (int)error.ToStatusCode();
        var errors = Validation.TryExtractFieldMap(error);
        
        return new ProblemShape(
            Title: error.Type.ToString(),
            Status: status,
            Type: $"urn:problem-type:{error.Type.ToString().ToLowerInvariant()}",
            Detail: error.Message,
            Instance: instance,
            Errors: errors
        );
    }
}