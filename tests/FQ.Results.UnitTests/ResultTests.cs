using FluentAssertions;

namespace FQ.Results.UnitTests;

public class ResultTests
{
    [Fact]
    public void Ok_IsSuccess_true_And_Error_null()
    {
        var r = Result.Ok();
        
        r.IsSuccess.Should().BeTrue();
        r.Error.Should().BeNull();
        r.ToString().Should().Be("Result: Ok");
    }

    [Fact]
    public void Fail_IsSuccess_false_And_Carries_Error()
    {
        var err = Error.Conflict("dupe", "Duplicate");
        var r = Result.Fail(err);

        r.IsSuccess.Should().BeFalse();
        r.Error.Should().Be(err);
        
        r.ToString().Should().Contain("Fail");
    }

    [Fact]
    public void Implicit_From_Error_Creates_Failure()
    {
        Result r = Error.Forbidden();
        
        r.IsSuccess.Should().BeFalse();
        r.Error!.Type.Should().Be(ErrorType.Forbidden);
    }

    [Fact]
    public void Ensure_Fails_When_Condition_False()
    {
        var r = Result.Ok().Ensure(() => false, () => Error.Validation("bad", "nope"));
        
        r.IsSuccess.Should().BeFalse();
        r.Error!.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void Tap_Runs_Action_On_Success()
    {
        var called = false;
        var r = Result.Ok().Tap(() => called = true);

        called.Should().BeTrue();
        r.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Combine_Returns_First_Failure()
    {
        var a = Result.Ok();
        var b = Result.Fail(Error.NotFound("x","y"));
        var c = Result.Ok();

        var combined = Result.Combine(a, b, c);
        
        combined.IsSuccess.Should().BeFalse();
        combined.Error!.Type.Should().Be(ErrorType.NotFound);
    }
}