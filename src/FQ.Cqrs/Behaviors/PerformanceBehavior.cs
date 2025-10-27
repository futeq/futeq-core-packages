using System.Diagnostics;
using FQ.Results;
using MediatR;

namespace FQ.Cqrs.Behaviors;

/// <summary>
/// Measures execution time and reports outcomes via <see cref="IRequestLogger"/>.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
public sealed class PerformanceBehavior<TRequest> : IPipelineBehavior<TRequest, Result> where TRequest : notnull
{
    private readonly IRequestLogger _logger;
    private readonly PerformanceOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceBehavior{TRequest}"/> class.
    /// </summary>
    /// <param name="logger">The request logger.</param>
    /// <param name="options">Performance options.</param>
    public PerformanceBehavior(IRequestLogger logger, PerformanceOptions options)
    {
        _logger = logger;
        _options = options;
    }

    /// <inheritdoc/>
    public async Task<Result> Handle(TRequest request, RequestHandlerDelegate<Result> next, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var traceId = _options.TraceIdFactory();
        
        _logger.Started(typeof(TRequest), traceId);

        var res = await next(ct);
        
        sw.Stop();

        if (!res.IsSuccess)
        {
            _logger.Failed(typeof(TRequest), traceId, res.Error!, sw.Elapsed);
        }
        else
        {
            _logger.Completed(typeof(TRequest), traceId, sw.Elapsed, true);
        }

        if (sw.Elapsed > _options.SlowThreshold)
        {
            _logger.SlowRequest(typeof(TRequest), traceId, sw.Elapsed);
        }

        return res;
    }
}

/// <summary>
/// Measures execution time and reports outcomes via <see cref="IRequestLogger"/>.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The successful response value type.</typeparam>
public sealed class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, Result<TResponse>> where TRequest : notnull
{
    private readonly IRequestLogger _logger;
    private readonly PerformanceOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The request logger.</param>
    /// <param name="options">Performance options.</param>
    public PerformanceBehavior(IRequestLogger logger, PerformanceOptions options)
    {
        _logger = logger;
        _options = options;
    }

    /// <inheritdoc/>
    public async Task<Result<TResponse>> Handle(TRequest request, RequestHandlerDelegate<Result<TResponse>> next, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var traceId = _options.TraceIdFactory();
        
        _logger.Started(typeof(TRequest), traceId);

        var res = await next(ct);
        
        sw.Stop();

        if (!res.IsSuccess)
        {
            _logger.Failed(typeof(TRequest), traceId, res.Error!, sw.Elapsed);
        }
        else
        {
            _logger.Completed(typeof(TRequest), traceId, sw.Elapsed, true);
        }

        if (sw.Elapsed > _options.SlowThreshold)
        {
            _logger.SlowRequest(typeof(TRequest), traceId, sw.Elapsed);
        }

        return res;
    }
}