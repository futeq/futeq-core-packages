using FluentAssertions;
using FQ.Cqrs.Behaviors;
using NSubstitute;

namespace FQ.Cqrs.UnitTests;

file sealed record GetValue(string Key) : IQuery<string>, ICacheableQuery
{
    public string CacheKey => $"val:{Key}";
    public CacheOptions CacheOptions => new() { Ttl = TimeSpan.FromMinutes(1), Tags = new[] { "vals" } };
}
file sealed record PutValue(string Key, string Val) : ICommand, ICacheInvalidation
{
    public IEnumerable<string> InvalidateTags() => new[] { "vals" };
}

public class CachingBehaviorTests
{
    [Fact]
    public async Task Should_Return_From_Cache_When_Hit()
    {
        var cache = Substitute.For<IQueryCache>();
        cache.TryGetAsync<string>("val:a", Arg.Any<CancellationToken>())!
            .Returns(Task.FromResult((true, "cached")));

        var behavior = new CachingBehavior<GetValue, string>(cache);

        var res = await behavior.Handle(new GetValue("a"), TestHelpers.NextOk("fresh"), default);

        res.IsSuccess.Should().BeTrue();
        res.Value.Should().Be("cached");
        await cache.DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CacheOptions>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_Set_Cache_On_Miss()
    {
        var cache = Substitute.For<IQueryCache>();
        cache.TryGetAsync<string>("val:a", Arg.Any<CancellationToken>())!
            .Returns(Task.FromResult((false, default(string)!)));

        var behavior = new CachingBehavior<GetValue, string>(cache);

        var res = await behavior.Handle(new GetValue("a"), TestHelpers.NextOk("fresh"), default);

        res.IsSuccess.Should().BeTrue();
        res.Value.Should().Be("fresh");
        await cache.Received(1).SetAsync("val:a", "fresh",
            Arg.Is<CacheOptions>(o => o.Ttl > TimeSpan.Zero && o.Tags.Contains("vals")), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Invalidation_Should_Fire_On_Successful_Command()
    {
        var cache = Substitute.For<IQueryCache>();
        var behavior = new CacheInvalidationBehavior<PutValue>(cache);

        var res = await behavior.Handle(new PutValue("a","1"), TestHelpers.NextOk(), default);

        res.IsSuccess.Should().BeTrue();
        await cache.Received(1).InvalidateByTagsAsync(Arg.Is<IEnumerable<string>>(t => t.Contains("vals")), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Invalidation_Should_Not_Fire_On_Failure()
    {
        var cache = Substitute.For<IQueryCache>();
        var behavior = new CacheInvalidationBehavior<PutValue>(cache);

        var res = await behavior.Handle(new PutValue("a","1"), TestHelpers.NextFail(), default);

        res.IsSuccess.Should().BeFalse();
        await cache.DidNotReceive().InvalidateByTagsAsync(Arg.Any<IEnumerable<string>>(), Arg.Any<CancellationToken>());
    }
}