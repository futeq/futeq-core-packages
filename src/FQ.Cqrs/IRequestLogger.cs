using FQ.Results;

namespace FQ.Cqrs;

/// <summary>
/// Minimal logging abstraction used by performance behavior to report timings and outcomes.
/// </summary>
public interface IRequestLogger
{
    /// <summary>
    /// Called when a request begins execution.
    /// </summary>
    /// <param name="requestType">The concrete request type.</param>
    /// <param name="traceId">A correlation or trace identifier.</param>
    void Started(Type requestType, string traceId);

    /// <summary>
    /// Called when a request completes successfully.
    /// </summary>
    /// <param name="requestType">The concrete request type.</param>
    /// <param name="traceId">A correlation or trace identifier.</param>
    /// <param name="elapsed">The elapsed execution time.</param>
    /// <param name="success">Should be <c>true</c> for successful completion.</param>
    void Completed(Type requestType, string traceId, TimeSpan elapsed, bool success);

    /// <summary>
    /// Called when a request exceeds the configured "slow" threshold.
    /// </summary>
    /// <param name="requestType">The concrete request type.</param>
    /// <param name="traceId">A correlation or trace identifier.</param>
    /// <param name="elapsed">The elapsed execution time.</param>
    void SlowRequest(Type requestType, string traceId, TimeSpan elapsed);

    /// <summary>
    /// Called when a request fails.
    /// </summary>
    /// <param name="requestType">The concrete request type.</param>
    /// <param name="traceId">A correlation or trace identifier.</param>
    /// <param name="error">The error returned by the handler pipeline.</param>
    /// <param name="elapsed">The elapsed execution time up to the failure.</param>
    void Failed(Type requestType, string traceId, Error error, TimeSpan elapsed);
}