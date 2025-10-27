using FQ.Results;

namespace FQ.Cqrs;

/// <summary>
/// Defines an authorizer for a specific request type.
/// </summary>
/// <typeparam name="TRequest">The request type to authorize.</typeparam>
public interface IAuthorizer<in TRequest>
{
    /// <summary>
    /// Performs authorization for the given request instance.
    /// </summary>
    /// <param name="request">The request to authorize.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>
    /// <see cref="Result.Ok()"/> when authorized; otherwise a failed <see cref="Result"/> with
    /// <see cref="ErrorType.Forbidden"/> or <see cref="ErrorType.Unauthorized"/>.
    /// </returns>
    Task<Result> AuthorizeAsync(TRequest request, CancellationToken ct);
}