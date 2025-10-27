namespace FQ.Cqrs;

/// <summary>
/// Dispatches domain events or flushes an outbox after a successful operation.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches any pending domain events or integration messages.
    /// </summary>
    /// <param name="ct">A cancellation token.</param>
    Task DispatchPendingAsync(CancellationToken ct);
}