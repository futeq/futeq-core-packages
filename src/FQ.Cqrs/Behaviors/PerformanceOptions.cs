using System.Diagnostics;

namespace FQ.Cqrs.Behaviors;

/// <summary>
/// Options for the performance behavior.
/// </summary>
public sealed class PerformanceOptions
{
    /// <summary>
    /// Gets or sets the threshold after which a request is considered slow. Defaults to 500ms.
    /// </summary>
    public TimeSpan SlowThreshold { get; init; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Gets or sets a factory that produces a trace identifier for the current request.
    /// </summary>
    public Func<string> TraceIdFactory { get; init; } = () => Activity.Current?.Id ?? Guid.NewGuid().ToString("N");
}