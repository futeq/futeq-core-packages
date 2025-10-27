using FluentAssertions;

namespace FQ.Results.UnitTests;

public class ResultOfTTests
{
    [Fact]
    public void Ok_Value_Present_And_Error_null()
    {
        var r = Result<int>.Ok(7);
        
        r.IsSuccess.Should().BeTrue();
        r.Value.Should().Be(7);
        r.Error.Should().BeNull();
        r.ToString().Should().Contain("Ok<Int32>");
    }

    [Fact]
    public void Fail_Carries_Error_And_No_Value()
    {
        var r = Result<int>.Fail(Error.Unauthorized());
        
        r.IsSuccess.Should().BeFalse();
        r.Value.Should().Be(0);
        
        r.Error!.Type.Should().Be(ErrorType.Unauthorized);
    }

    [Fact]
    public void Implicit_From_Value_And_From_Error_Work()
    {
        Result<string> ok = "hi";
        
        ok.IsSuccess.Should().BeTrue();
        ok.Value.Should().Be("hi");

        Result<string> fail = Error.Validation("v","bad");
        
        fail.IsSuccess.Should().BeFalse();
        fail.Error!.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public void TryGetValue_Works()
    {
        Result<string> ok = "x";
        
        ok.TryGetValue(out var v).Should().BeTrue();
        v.Should().Be("x");

        Result<string> fail = Error.NotFound("nf","no");
        
        fail.TryGetValue(out var _).Should().BeFalse();
    }

    [Fact]
    public void Map_Bind_Tap_Ensure_Pipelines()
    {
        Result<string> r = "abc";
        
        var mapped = r.Map(s => s.Length)
            .Ensure(len => len == 3, _ => Error.Validation("len", "bad"))
            .Tap(_ => {}) ;

        mapped.IsSuccess.Should().BeTrue();
        mapped.Value.Should().Be(3);

        Result<string> bad = Error.NotFound("x","y");
        
        var mappedBad = bad.Map(s => s.Length);
        
        mappedBad.IsSuccess.Should().BeFalse();
        mappedBad.Error!.Type.Should().Be(ErrorType.NotFound);
    }
}