using FluentAssertions;
using FQ.Cqrs.Behaviors;
using FQ.Results;
using NSubstitute;

namespace FQ.Cqrs.UnitTests;

file sealed record SensitiveCommand(string? OwnerId) : ICommand;

public class AuthorizationBehaviorTests
{
    [Fact]
    public async Task Should_ShortCircuit_On_Authorization_Failure()
    {
        var auth = Substitute.For<IAuthorizer<SensitiveCommand>>();
        
        auth.AuthorizeAsync(Arg.Any<SensitiveCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Fail(Error.Forbidden())));

        var behavior = new AuthorizationBehavior<SensitiveCommand>([auth]);

        var res = await behavior.Handle(new SensitiveCommand("u1"), TestHelpers.NextOk(), CancellationToken.None);

        res.IsSuccess.Should().BeFalse();
        res.Error!.Type.Should().Be(ErrorType.Forbidden);
        await auth.Received(1).AuthorizeAsync(Arg.Any<SensitiveCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Pass_When_All_Authorizers_Succeed()
    {
        var auth = Substitute.For<IAuthorizer<SensitiveCommand>>();
        auth.AuthorizeAsync(Arg.Any<SensitiveCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Ok()));

        var behavior = new AuthorizationBehavior<SensitiveCommand>(new[] { auth });

        var res = await behavior.Handle(new SensitiveCommand("u1"), TestHelpers.NextOk(), default);

        res.IsSuccess.Should().BeTrue();
    }
}