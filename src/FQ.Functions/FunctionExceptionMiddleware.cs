using System.Net;
using System.Text.Json;
using FQ.Results;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace FQ.Functions;

/// <summary>
/// Functions isolated worker middleware that converts unhandled exceptions to RFC7807 responses.
/// </summary>
public sealed class FunctionExceptionMiddleware : IFunctionsWorkerMiddleware
{
    private readonly JsonSerializerOptions _json;
    private readonly FunctionRequestResolver _resolve;

    public FunctionExceptionMiddleware(JsonSerializerOptions? json = null,
        FunctionRequestResolver? resolve = null)
    {
        _json = json ?? new(JsonSerializerDefaults.Web);
        _resolve = resolve ?? (async ctx => await ctx.GetHttpRequestDataAsync());
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var req = await _resolve(context);
            if (req is null) throw;

            var p = Error.FromException(ex).ToProblemShape(req.Url.AbsolutePath);
            var res = req.CreateResponse((HttpStatusCode)p.Status);
            
            res.Headers.Add("Content-Type", "application/problem+json");
            
            await res.WriteStringAsync(JsonSerializer.Serialize(p, _json));
            
            context.GetInvocationResult().Value = res;
        }
    }
}