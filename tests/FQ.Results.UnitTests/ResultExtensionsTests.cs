using FluentAssertions;

namespace FQ.Results.UnitTests;

public class ResultExtensionsTests
{
    [Fact]
    public void Collect_Combines_To_Array_On_Success()
    {
        var list = new[] { 1, 2, (Result<int>)3 };
        var collected = list.Collect();

        collected.IsSuccess.Should().BeTrue();
        collected.Value.Should().Equal(1,2,3);
    }

    [Fact]
    public void Collect_Stops_On_First_Failure()
    {
        var list = new[] { (Result<int>)1, Result<int>.Fail(Error.Conflict("x","y")), (Result<int>)3 };
        var collected = list.Collect();

        collected.IsSuccess.Should().BeFalse();
        collected.Error!.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public void CombineAll_Returns_First_Failure()
    {
        var res = new[] { Result.Ok(), Result.Fail(Error.Forbidden()), Result.Ok() }.CombineAll();
        res.IsSuccess.Should().BeFalse();
        res.Error!.Type.Should().Be(ErrorType.Forbidden);
    }
}