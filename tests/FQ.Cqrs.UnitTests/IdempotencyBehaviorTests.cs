using System.Text.Json;
using FluentAssertions;
using FQ.Cqrs.Behaviors;
using FQ.Results;
using NSubstitute;

namespace FQ.Cqrs.UnitTests;

file sealed record MakePayment(decimal Amount) : ICommand<Guid>, IIdempotentRequest
{
    public TimeSpan? IdempotencyTtl => TimeSpan.FromMinutes(5);
}

public class IdempotencyBehaviorTests
{
    [Fact]
    public async Task Should_Return_Cached_Generic_Result_When_Found()
    {
        var keyAccessor = Substitute.For<IIdempotencyKeyAccessor>();
        
        keyAccessor.GetKey().Returns("KEY1");

        var store = Substitute.For<IIdempotencyStore>();
        var options = new IdempotencyOptions();

        var expected = Result<Guid>.Ok(Guid.NewGuid());
        var payload = JsonSerializer.SerializeToUtf8Bytes(expected, TestHelpers.Json);
        
        store.TryGetAsync("KEY1", typeof(MakePayment).FullName!, Arg.Any<CancellationToken>())!
            .Returns(Task.FromResult((true, payload, "application/json")));

        var behavior = new IdempotencyBehavior<MakePayment, Guid>(keyAccessor, store, options);
        var res = await behavior.Handle(new MakePayment(10), TestHelpers.NextOk(Guid.NewGuid()), default);

        res.IsSuccess.Should().BeTrue();
        res.Value.Should().Be(expected.Value);
        
        await store.DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<string?>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Store_Result_When_Not_Found()
    {
        var keyAccessor = Substitute.For<IIdempotencyKeyAccessor>();
        
        keyAccessor.GetKey().Returns("KEY2");

        var store = Substitute.For<IIdempotencyStore>();
        
        store.TryGetAsync("KEY2", typeof(MakePayment).FullName!, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((false, null as byte[], null as string)));

        var behavior = new IdempotencyBehavior<MakePayment, Guid>(keyAccessor, store, new IdempotencyOptions());

        var id = Guid.NewGuid();
        var res = await behavior.Handle(new MakePayment(25), TestHelpers.NextOk(id), default);

        res.IsSuccess.Should().BeTrue();
        
        await store.Received(1).SetAsync("KEY2", typeof(MakePayment).FullName!,
            Arg.Any<byte[]>(), "application/json", TimeSpan.FromMinutes(5), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Nongeneric_Should_Work_Same_Way()
    {
        var keyAccessor = Substitute.For<IIdempotencyKeyAccessor>();
        
        keyAccessor.GetKey().Returns("K");

        var store = Substitute.For<IIdempotencyStore>();
        
        store.TryGetAsync("K", typeof(DoSomething).FullName!, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((false, null as byte[], null as string)));

        var behavior = new IdempotencyBehavior<DoSomething>(keyAccessor, store, new IdempotencyOptions());

        var res = await behavior.Handle(new DoSomething(), TestHelpers.NextOk(), default);

        res.IsSuccess.Should().BeTrue();
        
        await store.Received(1).SetAsync("K", typeof(DoSomething).FullName!, Arg.Any<byte[]>(),
            "application/json", TimeSpan.FromHours(24), Arg.Any<CancellationToken>());
    }

    private sealed record DoSomething : ICommand, IIdempotentRequest
    {
        public TimeSpan? IdempotencyTtl => null; // use default
    }
}