namespace FQ.Cqrs;

/// <summary>
/// Marker interface indicating that a command should not be wrapped in a transaction.
/// </summary>
public interface INonTransactional { }