using FluentAssertions;
using FQ.Cqrs.Behaviors;
using FQ.Results;
using NSubstitute;

namespace FQ.Cqrs.UnitTests;

public sealed record SensitiveCommand(string? OwnerId) : ICommand;

public class AuthorizationBehaviorTests
{
    [Fact]
    public async Task Should_ShortCircuit_On_Authorization_Failure()
    {
        var auth = Substitute.For<IAuthorizer<SensitiveCommand>>();
        auth.AuthorizeAsync(Arg.Any<SensitiveCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Fail(Error.Forbidden())));

        var behavior = new AuthorizationBehavior<SensitiveCommand, int>(new[] { auth });

        var res = await behavior.Handle(
            new SensitiveCommand("u1"),
            // next (will never be hit)
            (CancellationToken ct) => Task.FromResult(Result<int>.Ok(42)),
            CancellationToken.None);

        res.IsSuccess.Should().BeFalse();
        res.Error!.Type.Should().Be(ErrorType.Forbidden);
        await auth.Received(1).AuthorizeAsync(Arg.Any<SensitiveCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Pass_When_All_Authorizers_Succeed()
    {
        var auth1 = Substitute.For<IAuthorizer<SensitiveCommand>>();
        var auth2 = Substitute.For<IAuthorizer<SensitiveCommand>>();

        auth1.AuthorizeAsync(Arg.Any<SensitiveCommand>(), Arg.Any<CancellationToken>())
             .Returns(Task.FromResult(Result.Ok()));
        auth2.AuthorizeAsync(Arg.Any<SensitiveCommand>(), Arg.Any<CancellationToken>())
             .Returns(Task.FromResult(Result.Ok()));

        var behavior = new AuthorizationBehavior<SensitiveCommand>(new[] { auth1, auth2 });

        var res = await behavior.Handle(
            new SensitiveCommand("u1"),
            (CancellationToken ct) => Task.FromResult(Result.Ok()),
            CancellationToken.None);

        res.IsSuccess.Should().BeTrue();
        await auth1.Received(1).AuthorizeAsync(Arg.Any<SensitiveCommand>(), Arg.Any<CancellationToken>());
        await auth2.Received(1).AuthorizeAsync(Arg.Any<SensitiveCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task NoAuthorizers_Configured_Should_PassThrough()
    {
        var behavior = new AuthorizationBehavior<SensitiveCommand>(Array.Empty<IAuthorizer<SensitiveCommand>>());

        var res = await behavior.Handle(
            new SensitiveCommand("u1"),
            (CancellationToken ct) => Task.FromResult(Result.Ok()),
            CancellationToken.None);

        res.IsSuccess.Should().BeTrue();
    }
}