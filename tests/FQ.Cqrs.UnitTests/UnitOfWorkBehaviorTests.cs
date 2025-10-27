using FluentAssertions;
using FQ.Cqrs.Behaviors;
using NSubstitute;

namespace FQ.Cqrs.UnitTests;

file sealed record ChangeEmail(Guid UserId, string Email) : ICommand;
file sealed record QueryUser(Guid UserId) : IQuery<string>;

public class UnitOfWorkBehaviorTests
{
    [Fact]
    public async Task Should_Begin_Commit_On_Command_Success()
    {
        var uow = Substitute.For<IUnitOfWork>();
        var behavior = new UnitOfWorkBehavior<ChangeEmail>(uow);

        var res = await behavior.Handle(new ChangeEmail(Guid.NewGuid(), "a@b.com"), TestHelpers.NextOk(), default);

        res.IsSuccess.Should().BeTrue();
        Received.InOrder(() =>
        {
            uow.BeginAsync(Arg.Any<CancellationToken>());
            uow.CommitAsync(Arg.Any<CancellationToken>());
        });
        await uow.DidNotReceive().RollbackAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Rollback_On_Command_Failure()
    {
        var uow = Substitute.For<IUnitOfWork>();
        var behavior = new UnitOfWorkBehavior<ChangeEmail>(uow);

        var res = await behavior.Handle(new ChangeEmail(Guid.NewGuid(), "bad"), TestHelpers.NextFail(), default);

        res.IsSuccess.Should().BeFalse();
        await uow.Received(1).BeginAsync(Arg.Any<CancellationToken>());
        await uow.Received(1).RollbackAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Not_Wrap_Queries()
    {
        var uow = Substitute.For<IUnitOfWork>();
        var behavior = new UnitOfWorkBehavior<QueryUser>(uow);

        var res = await behavior.Handle(new QueryUser(Guid.NewGuid()), TestHelpers.NextOk(), default);

        res.IsSuccess.Should().BeTrue();
        await uow.DidNotReceive().BeginAsync(Arg.Any<CancellationToken>());
    }
}