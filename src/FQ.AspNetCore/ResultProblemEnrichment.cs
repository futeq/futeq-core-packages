using FQ.Results;
using Microsoft.AspNetCore.Http;

namespace FQ.AspNetCore;

/// <summary>Helpers to enrich RFC7807 output with correlation ID.</summary>
public static class ResultProblemEnrichment
{
    /// <summary>Creates a problem extension dictionary including correlation ID if present.</summary>
    public static IDictionary<string, object?>? WithCorrelationExtensions(this ProblemShape p, HttpContext http, string header = "X-Correlation-Id")
    {
        var cid = http.GetCorrelationId(header);
        if (cid is null && p.Errors is null) return null;

        var dict = new Dictionary<string, object?>(2);
        if (p.Errors is not null) dict["errors"] = p.Errors;
        if (cid is not null) dict["correlationId"] = cid;
        return dict;
    }
}