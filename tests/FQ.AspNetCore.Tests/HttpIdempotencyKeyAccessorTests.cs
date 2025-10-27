using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace FQ.AspNetCore.Tests;

public class HttpIdempotencyKeyAccessorTests
{
    [Fact]
    public void Reads_Idempotency_Key_From_Header()
    {
        var http = new DefaultHttpContext();
        
        http.Request.Headers["Idempotency-Key"] = "IDEMP-1";
        
        var accessor = new HttpIdempotencyKeyAccessor(new HttpContextAccessor { HttpContext = http });

        accessor.GetKey().Should().Be("IDEMP-1");
    }

    [Fact]
    public void Returns_Null_When_Missing()
    {
        var http = new DefaultHttpContext();
        var accessor = new HttpIdempotencyKeyAccessor(new HttpContextAccessor { HttpContext = http });

        accessor.GetKey().Should().BeNull();
    }
}