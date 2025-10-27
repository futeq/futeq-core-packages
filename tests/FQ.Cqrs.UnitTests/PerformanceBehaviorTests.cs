using FluentAssertions;
using FQ.Cqrs.Behaviors;
using FQ.Results;
using NSubstitute;

namespace FQ.Cqrs.UnitTests;

file sealed record Ping : ICommand;

public class PerformanceBehaviorTests
{
    [Fact]
    public async Task Should_Log_Started_Completed_And_Slow()
    {
        var logger = Substitute.For<IRequestLogger>();
        var opts = new PerformanceOptions { SlowThreshold = TimeSpan.Zero }; // always slow
        var behavior = new PerformanceBehavior<Ping>(logger, opts);

        var res = await behavior.Handle(new Ping(), TestHelpers.NextOk(), default);

        res.IsSuccess.Should().BeTrue();
        logger.Received().Started(typeof(Ping), Arg.Any<string>());
        logger.Received().Completed(typeof(Ping), Arg.Any<string>(), Arg.Any<TimeSpan>(), true);
        logger.Received().SlowRequest(typeof(Ping), Arg.Any<string>(), Arg.Any<TimeSpan>());
    }

    [Fact]
    public async Task Should_Log_Failed()
    {
        var logger = Substitute.For<IRequestLogger>();
        var opts = new PerformanceOptions { SlowThreshold = TimeSpan.FromDays(1) };
        var behavior = new PerformanceBehavior<Ping>(logger, opts);

        var res = await behavior.Handle(new Ping(), TestHelpers.NextFail(), default);

        res.IsSuccess.Should().BeFalse();
        logger.Received().Failed(typeof(Ping), Arg.Any<string>(), Arg.Any<Error>(), Arg.Any<TimeSpan>());
    }
}