using System.Net;
using System.Text.Json;
using FluentAssertions;
using FQ.Functions.Testing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace FQ.Functions.UnitTests;

public class FunctionExceptionMiddlewareTests
{
    [Fact]
    public async Task Converts_Exception_To_ProblemJson()
    {
        var ctx = FunctionTestingHelper.Ctx();
        var (req, _) = FunctionTestingHelper.Http(ctx);

        var mw = new FunctionExceptionMiddleware(resolve: _ => Task.FromResult<HttpRequestData?>(req));

        await mw.Invoking(m => m.Invoke(ctx, _ => throw new InvalidOperationException("boom")))
            .Should().NotThrowAsync();

        var result = ctx.GetInvocationResult().Value as HttpResponseData;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Headers.GetValues("Content-Type").Single().Should().Be("application/problem+json");
        result.Body.Position = 0;
        var json = await new StreamReader(result.Body).ReadToEndAsync();
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.GetProperty("status").GetInt32().Should().Be(400);
    }

    [Fact]
    public async Task NonHttp_Rethrows()
    {
        var ctx = FunctionTestingHelper.Ctx();
        var mw = new FunctionExceptionMiddleware(resolve: _ => Task.FromResult<HttpRequestData?>(null));

        await mw.Invoking(m => m.Invoke(ctx, _ => throw new InvalidOperationException("boom")))
            .Should().ThrowAsync<InvalidOperationException>();
    }
}