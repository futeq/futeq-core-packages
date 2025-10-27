using FQ.Results;
using MediatR;

namespace FQ.Cqrs;

/// <summary>
/// Marker interface for query requests that return a value.
/// </summary>
/// <typeparam name="TResponse">The value returned when the query succeeds.</typeparam>
/// <remarks>
/// Queries represent read-only operations. Handlers should return a <see cref="Futeq.Results.Result{T}"/>.
/// </remarks>
public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }