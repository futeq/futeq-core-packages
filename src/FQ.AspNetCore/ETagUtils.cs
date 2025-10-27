using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace FQ.AspNetCore;

/// <summary>
/// Utilities for working with ETag/If-None-Match headers.
/// </summary>
public static class ETagUtils
{
    /// <summary>
    /// Computes a weak ETag (W/) from a byte payload.
    /// </summary>
    public static string ComputeWeakETag(ReadOnlySpan<byte> bytes)
    {
        var hash = SHA256.HashData(bytes);
        return $"W/\"{Convert.ToHexString(hash)}\"";
    }

    /// <summary>
    /// Computes a weak ETag from a string payload.
    /// </summary>
    public static string ComputeWeakETag(string text)
        => ComputeWeakETag(Encoding.UTF8.GetBytes(text));

    /// <summary>
    /// Checks If-None-Match and writes 304 Not Modified when matching the provided ETag.
    /// </summary>
    public static bool TryRespondNotModified(HttpContext ctx, string etag)
    {
        var inm = ctx.Request.Headers.IfNoneMatch.ToString();
        if (string.IsNullOrWhiteSpace(inm)) return false;

        if (inm.Split(',').Select(s => s.Trim()).Any(s => string.Equals(s, etag, StringComparison.Ordinal)))
        {
            ctx.Response.StatusCode = StatusCodes.Status304NotModified;
            ctx.Response.Headers.ETag = etag;
            return true;
        }
        return false;
    }
}