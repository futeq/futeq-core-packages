using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace FQ.AspNetCore.Tests;

public class ETagUtilsTests
{
    [Fact]
    public void ComputeWeakETag_Is_Deterministic()
    {
        var a = ETagUtils.ComputeWeakETag("hello");
        var b = ETagUtils.ComputeWeakETag(Encoding.UTF8.GetBytes("hello"));
        a.Should().Be(b);
        a.Should().StartWith("W/\"");
    }

    [Fact]
    public void TryRespondNotModified_Sets_304_When_Match()
    {
        var ctx = new DefaultHttpContext();
        var etag = ETagUtils.ComputeWeakETag("payload");
        ctx.Request.Headers.IfNoneMatch = etag;

        var notModified = ETagUtils.TryRespondNotModified(ctx, etag);

        notModified.Should().BeTrue();
        ctx.Response.StatusCode.Should().Be(StatusCodes.Status304NotModified);
        ctx.Response.Headers.ETag.ToString().Should().Be(etag);
    }
}