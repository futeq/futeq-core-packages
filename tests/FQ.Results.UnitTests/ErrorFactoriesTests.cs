using FluentAssertions;

namespace FQ.Results.UnitTests;


public class ErrorFactoriesTests
{
    [Fact]
    public void FromException_Without_Stack()
    {
        var ex = new InvalidOperationException("boom");
        var e = Error.FromException(ex);

        e.Code.Should().Be("exception");
        e.Message.Should().Be("boom");
        e.Metadata.Should().BeNull(); // no stack
    }

    [Fact]
    public void FromException_With_Stack()
    {
        var ex = new InvalidOperationException("kaboom");
        var e = Error.FromException(ex, includeStack: true);

        e.Metadata.Should().NotBeNull();
        e.Metadata!.Should().ContainKey("exception");
        e.Metadata!.Should().ContainKey("stack");
    }
}