namespace FQ.Cqrs;

/// <summary>
/// Default implementation of <see cref="IClock"/> using <see cref="DateTime.UtcNow"/>.
/// </summary>
public sealed class SystemClock : IClock
{
    /// <inheritdoc/>
    public DateTime UtcNow => DateTime.UtcNow;
}