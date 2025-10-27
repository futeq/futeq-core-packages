using System.Text.Json;
using FQ.Results;
using Microsoft.AspNetCore.Http;

namespace FQ.AspNetCore;

/// <summary>
/// Minimal exception middleware that converts unhandled exceptions to RFC7807 JSON using <see cref="Error.FromException"/>.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JsonSerializerOptions _json;

    /// <summary>
    /// Creates the middleware instance.
    /// </summary>
    public ExceptionHandlingMiddleware(RequestDelegate next, JsonSerializerOptions? json = null)
    {
        _next = next;
        _json = json ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    /// <summary>
    /// Executes the middleware.
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // standard cancellation path; let server handle it
            throw;
        }
        catch (Exception ex)
        {
            var err = Error.FromException(ex);
            var p = err.ToProblemShape(context.Request?.Path.Value);

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = p.Status;
            await context.Response.WriteAsync(JsonSerializer.Serialize(p, _json));
        }
    }
}