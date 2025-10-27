namespace FQ.Cqrs;

/// <summary>
/// Unit of Work abstraction used by the transaction behavior.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Begins a new transaction scope if one is not already active.
    /// </summary>
    /// <param name="ct">A cancellation token.</param>
    Task BeginAsync(CancellationToken ct);

    /// <summary>
    /// Commits the current transaction scope.
    /// </summary>
    /// <param name="ct">A cancellation token.</param>
    Task CommitAsync(CancellationToken ct);

    /// <summary>
    /// Rolls back the current transaction scope.
    /// </summary>
    /// <param name="ct">A cancellation token.</param>
    Task RollbackAsync(CancellationToken ct);

    /// <summary>
    /// Gets a value indicating whether a transaction scope is currently active.
    /// </summary>
    bool HasActiveTransaction { get; }
}