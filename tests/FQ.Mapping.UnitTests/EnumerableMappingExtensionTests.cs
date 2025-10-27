using FluentAssertions;

namespace FQ.Mapping.UnitTests;

public class EnumerableMappingExtensionsTests
{
    [Fact]
    public void MapEach_Should_Map_Sequence()
    {
        var mapper = new FakeMapper();
        var src = new[]
        {
            new User{ Id=1, Name="A" },
            new User{ Id=2, Name="B" }
        };

        var dest = mapper.MapEach<User, UserDto>(src).ToList();

        dest.Should().HaveCount(2);
        dest.Select(d => d.Name).Should().Equal("A", "B");
    }
}