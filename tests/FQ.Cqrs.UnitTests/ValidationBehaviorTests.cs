using FluentAssertions;
using FQ.Cqrs.Behaviors;
using FQ.Results;
using FluentValidation;

namespace FQ.Cqrs.UnitTests;

file sealed record CreateUser(string? Name) : ICommand<Guid>;
file sealed class CreateUserValidator : AbstractValidator<CreateUser>
{
    public CreateUserValidator() => RuleFor(x => x.Name).NotEmpty();
}

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Generic_Should_Return_Validation_Error_On_Fail()
    {
        var validators = new IValidator<CreateUser>[] { new CreateUserValidator() };
        var behavior = new ValidationBehavior<CreateUser, Guid>(validators);

        var res = await behavior.Handle(new CreateUser(""), TestHelpers.NextOk(Guid.NewGuid()), default);

        res.IsSuccess.Should().BeFalse();
        res.Error!.Type.Should().Be(ErrorType.Validation);
        Validation.TryExtractFieldMap(res.Error!)!.Keys.Should().Contain("Name");
    }

    [Fact]
    public async Task Generic_Should_Pass_When_Valid()
    {
        var validators = new IValidator<CreateUser>[] { new CreateUserValidator() };
        var behavior = new ValidationBehavior<CreateUser, Guid>(validators);

        var id = Guid.NewGuid();
        var res = await behavior.Handle(new CreateUser("ok"), TestHelpers.NextOk(id), default);

        res.IsSuccess.Should().BeTrue();
        res.Value.Should().Be(id);
    }

    [Fact]
    public async Task Nongeneric_Should_ShortCircuit_On_Validation_Error()
    {
        var validators = new IValidator<DeleteUser>[] { new DeleteUserValidator() };
        var behavior = new ValidationBehavior<DeleteUser>(validators);

        var res = await behavior.Handle(new DeleteUser(Guid.Empty), TestHelpers.NextOk(), default);

        res.IsSuccess.Should().BeFalse();
        res.Error!.Type.Should().Be(ErrorType.Validation);
    }

    // supporting command
    private sealed record DeleteUser(Guid Id) : ICommand;
    private sealed class DeleteUserValidator : AbstractValidator<DeleteUser>
    {
        public DeleteUserValidator() => RuleFor(x => x.Id).NotEqual(Guid.Empty);
    }
}