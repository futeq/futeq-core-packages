using System.Net;
using System.Text.Json;
using FluentAssertions;
using FQ.Functions.Testing;
using FQ.Results;

namespace FQ.Functions.UnitTests;

public class HttpResponseDataExtensionsTests
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task NonGeneric_Success_204()
    {
        var ctx = FunctionTestingHelper.Ctx();
        var (req, _) = FunctionTestingHelper.Http(ctx);

        var res = await req.WriteResultAsync(Result.Ok(), Json);
        res.StatusCode.Should().Be(HttpStatusCode.NoContent);
        res.Headers.TryGetValues("Content-Type", out _).Should().BeFalse();
    }

    [Fact]
    public async Task Generic_Success_200_Json()
    {
        var ctx = FunctionTestingHelper.Ctx();
        var (req, _) = FunctionTestingHelper.Http(ctx);

        var res = await req.WriteResultAsync(Result<object>.Ok(new { id = 1, name = "Ada" }), Json);

        res.StatusCode.Should().Be(HttpStatusCode.OK);
        res.Headers.GetValues("Content-Type").Single().Should().StartWith("application/json");
        res.Body.Position = 0;
        var body = await new StreamReader(res.Body).ReadToEndAsync();
        body.Should().Contain("\"id\":1").And.Contain("\"name\":\"Ada\"");
    }

    [Fact]
    public async Task Failure_ProblemJson_And_Correlation_Echo()
    {
        var ctx = FunctionTestingHelper.Ctx();
        var (req, _) = FunctionTestingHelper.Http(ctx);
        req.Headers.Add("X-Correlation-Id", "CID-777");

        var res = await req.WriteResultAsync(Result.Fail(Error.NotFound("nf","missing")), Json);

        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
        res.Headers.GetValues("Content-Type").Single().Should().Be("application/problem+json");
        res.Headers.TryGetValues("X-Correlation-Id", out var vals).Should().BeTrue();
        vals!.Single().Should().Be("CID-777");
    }
}