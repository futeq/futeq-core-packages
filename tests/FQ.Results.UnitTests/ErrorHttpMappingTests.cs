using System.Net;
using FluentAssertions;

namespace FQ.Results.UnitTests;

public class ErrorHttpMappingTests
{
    public static IEnumerable<object[]> StatusCases() =>
    [
        [Error.Validation("v","bad"), HttpStatusCode.BadRequest],
        [Error.NotFound("nf","no"), HttpStatusCode.NotFound],
        [Error.Conflict("c","dup"), HttpStatusCode.Conflict],
        [Error.Unauthorized(), HttpStatusCode.Unauthorized],
        [Error.Forbidden(), HttpStatusCode.Forbidden],
        [Error.TooMany(), (HttpStatusCode)429]
    ];

    [Theory]
    [MemberData(nameof(StatusCases))]
    public void ToStatusCode_Maps_As_Expected(Error e, HttpStatusCode expected)
        => e.ToStatusCode().Should().Be(expected);

    [Fact]
    public void ToProblemShape_Fills_Fields()
    {
        var e = Error.Validation("v","Validation failed");
        var p = e.ToProblemShape("/orders/1");

        p.Title.Should().Be("Validation");
        p.Status.Should().Be((int)HttpStatusCode.BadRequest);
        p.Detail.Should().Be("Validation failed");
        p.Instance.Should().Be("/orders/1");
        p.Type.Should().Contain("validation");
    }

    [Fact]
    public void Success_Result_ToProblemShape_Is_OK()
    {
        var p = Result.Ok().ToProblemShape();
        
        p.Status.Should().Be((int)HttpStatusCode.OK);
        p.Title.Should().Be("OK");
    }
}