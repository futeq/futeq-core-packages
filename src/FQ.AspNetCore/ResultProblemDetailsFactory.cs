using System.Net;
using FQ.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace FQ.AspNetCore;

/// <summary>
/// Creates RFC7807 <see cref="ProblemDetails"/> from <see cref="Error"/>/ <see cref="Result"/> using <see cref="ErrorHttpMapping"/>.
/// Plug this in to unify API error output.
/// </summary>
public sealed class ResultProblemDetailsFactory : ProblemDetailsFactory
{
    private readonly ApiBehaviorOptions _options;

    /// <summary>
    /// Initializes a new instance of the factory.
    /// </summary>
    public ResultProblemDetailsFactory(IOptions<ApiBehaviorOptions> options)
        => _options = options.Value;

    /// <inheritdoc/>
    public override ProblemDetails CreateProblemDetails(
        Microsoft.AspNetCore.Http.HttpContext httpContext, int? statusCode = null,
        string? title = null, string? type = null, string? detail = null, string? instance = null)
    {
        var pd = new ProblemDetails
        {
            Status = statusCode ?? (int)HttpStatusCode.BadRequest,
            Title = title ?? "An error occurred.",
            Type = type ?? "about:blank",
            Detail = detail,
            Instance = instance ?? httpContext?.Request?.Path.Value
        };

        ApplyDefaults(httpContext, pd.Status!.Value, pd);
        return pd;
    }

    public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext,
        ModelStateDictionary modelStateDictionary, int? statusCode = null, string? title = null, string? type = null,
        string? detail = null, string? instance = null)
    {
        var v = new ValidationProblemDetails(modelStateDictionary)
        {
            Status = statusCode ?? (int)HttpStatusCode.BadRequest,
            Title = title ?? "Validation failed.",
            Type = type ?? "urn:problem-type:validation",
            Detail = detail,
            Instance = instance ?? httpContext?.Request?.Path.Value
        };

        ApplyDefaults(httpContext, v.Status!.Value, v);
        return v;
    }

    private void ApplyDefaults(Microsoft.AspNetCore.Http.HttpContext? httpContext, int statusCode, ProblemDetails problemDetails)
    {
        problemDetails.Status ??= statusCode;
        if (_options.ClientErrorMapping.TryGetValue(statusCode, out var kv))
        {
            problemDetails.Title ??= kv.Title;
            problemDetails.Type ??= kv.Link;
        }
    }
}