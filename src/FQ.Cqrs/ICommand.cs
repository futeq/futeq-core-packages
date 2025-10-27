using FQ.Results;
using MediatR;

namespace FQ.Cqrs;

/// <summary>
/// Marker interface for command requests that do not return a value.
/// </summary>
/// <remarks>
/// Commands represent state-changing operations. Handlers should return a <see cref="Futeq.Results.Result"/>.
/// </remarks>
public interface ICommand : IRequest<Result> { }

/// <summary>
/// Marker interface for command requests that return a value.
/// </summary>
/// <typeparam name="TResponse">The value returned when the command succeeds.</typeparam>
/// <remarks>
/// Commands represent state-changing operations. Handlers should return a <see cref="Futeq.Results.Result{T}"/>.
/// </remarks>
public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }