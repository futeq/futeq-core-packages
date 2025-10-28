using FluentAssertions;
using FQ.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FQ.AspNetCore.Tests;

public class ResultActionResultsTests
{
    private static ControllerBase MakeController(out DefaultHttpContext http)
    {
        http = new DefaultHttpContext();
        var ctrl = new TestController { ControllerContext = new ControllerContext { HttpContext = http } };
        return ctrl;
    }

    private sealed class TestController : ControllerBase { }

    [Fact]
    public void ToActionResult_NonGeneric_Success_NoContent()
    {
        var ctrl = MakeController(out _);
        var ar = Result.Ok().ToActionResult(ctrl);

        ar.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public void ToActionResult_Generic_Failure_Returns_ProblemDetails_With_Extensions()
    {
        var ctrl = MakeController(out var http);
        http.Request.Headers["X-Correlation-Id"] = "CID-123";

        var res = Result<string>.Fail(Error.Validation("name", "Required"));
        var ar = res.ToActionResult(ctrl);

        var obj = Assert.IsType<ObjectResult>(ar.Result);
        obj.StatusCode.Should().Be(400);

        var problem = Assert.IsType<ProblemDetails>(obj.Value);
        problem.Type.Should().Contain("validation");
        problem.Extensions.Should().ContainKey("correlationId");
        problem.Extensions["correlationId"].Should().Be("CID-123");
    }

    [Fact]
    public void ToHttpResult_NonGeneric_Failure_Produces_Problem()
    {
        var http = new DefaultHttpContext();
        var r = Result.Fail(Error.NotFound("nf","no"));

        var ires = r.ToHttpResult(http);
        var obj = Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>(ires);
        obj.StatusCode.Should().Be(404);
    }
}