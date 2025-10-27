using FluentAssertions;
using FQ.Cqrs.Behaviors;
using NSubstitute;

namespace FQ.Cqrs.UnitTests;

file sealed record DoWork : ICommand;

public class DomainEventsBehaviorTests
{
    [Fact]
    public async Task Should_Dispatch_On_Success()
    {
        var dispatcher = Substitute.For<IDomainEventDispatcher>();
        var behavior = new DomainEventsBehavior<DoWork>(dispatcher);

        var res = await behavior.Handle(new DoWork(), TestHelpers.NextOk(), default);

        res.IsSuccess.Should().BeTrue();
        await dispatcher.Received(1).DispatchPendingAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Not_Dispatch_On_Failure()
    {
        var dispatcher = Substitute.For<IDomainEventDispatcher>();
        var behavior = new DomainEventsBehavior<DoWork>(dispatcher);

        var res = await behavior.Handle(new DoWork(), TestHelpers.NextFail(), default);

        res.IsSuccess.Should().BeFalse();
        await dispatcher.DidNotReceive().DispatchPendingAsync(Arg.Any<CancellationToken>());
    }
}