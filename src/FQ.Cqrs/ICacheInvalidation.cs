namespace FQ.Cqrs;

/// <summary>
/// Indicates that a command invalidates cache entries associated with specific tags upon success.
/// </summary>
public interface ICacheInvalidation
{
    /// <summary>
    /// Gets the set of tags that should be invalidated after a successful command.
    /// </summary>
    /// <returns>A sequence of cache tags.</returns>
    IEnumerable<string> InvalidateTags();
}