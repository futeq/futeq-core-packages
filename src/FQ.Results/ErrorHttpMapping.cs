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
        ErrorType.Validation => HttpStatusCode.BadRequest,
        ErrorType.NotFound => HttpStatusCode.NotFound,
        ErrorType.Conflict => HttpStatusCode.Conflict,
        ErrorType.Unauthorized => HttpStatusCode.Unauthorized,
        ErrorType.Forbidden => HttpStatusCode.Forbidden,
        ErrorType.PreconditionFailed => HttpStatusCode.PreconditionFailed,
        ErrorType.TooManyRequests => (HttpStatusCode)429,
        ErrorType.Concurrency => HttpStatusCode.Conflict,
        _ => HttpStatusCode.BadRequest
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