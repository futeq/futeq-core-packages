using System.Net;
using System.Text;
using System.Text.Json;
using FQ.Results;
using Microsoft.Azure.Functions.Worker.Http;

namespace FQ.Functions;

public static class HttpResponseDataExtensions
{
    public static async Task<HttpResponseData> WriteResultAsync(this HttpRequestData req, Result result, JsonSerializerOptions? json = null)
    {
        var res = req.CreateResponse();
        if (result.IsSuccess)
        {
            res.StatusCode = HttpStatusCode.NoContent;
            return res;
        }

        var p = result.ToProblemShape(req.Url.AbsolutePath);
        res.StatusCode = (HttpStatusCode)p.Status;
        res.Headers.Add("Content-Type", "application/problem+json");

        // Include correlation id if present on request
        if (req.Headers.TryGetValues("X-Correlation-Id", out var vals))
            res.Headers.Add("X-Correlation-Id", vals);

        await res.WriteStringAsync(JsonSerializer.Serialize(p, json ?? new(JsonSerializerDefaults.Web)));
        return res;
    }

    public static async Task<HttpResponseData> WriteResultAsync<T>(this HttpRequestData req, Result<T> result, JsonSerializerOptions? json = null)
    {
        var res = req.CreateResponse();
        var opts = json ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);

        if (result.IsSuccess)
        {
            res.StatusCode = HttpStatusCode.OK;
            res.Headers.Add("Content-Type", "application/json; charset=utf-8");
            if (req.Headers.TryGetValues("X-Correlation-Id", out var vals))
                res.Headers.Add("X-Correlation-Id", vals);
            await res.WriteStringAsync(JsonSerializer.Serialize(result.Value, opts), Encoding.UTF8);
            return res;
        }

        var p = result.ToProblemShape(req.Url.AbsolutePath);
        res.StatusCode = (HttpStatusCode)p.Status;
        res.Headers.Add("Content-Type", "application/problem+json");
        if (req.Headers.TryGetValues("X-Correlation-Id", out var vals2))
            res.Headers.Add("X-Correlation-Id", vals2);
        await res.WriteStringAsync(JsonSerializer.Serialize(p, opts), Encoding.UTF8);
        return res;
    }
}