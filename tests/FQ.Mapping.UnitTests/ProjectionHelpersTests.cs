using FluentAssertions;

namespace FQ.Mapping.UnitTests;

public class ProjectionHelpersTests
{
    [Fact]
    public void ProjectToInMemory_UnTyped_Works()
    {
        var mapper = new FakeMapper();
        IQueryable source = new List<User>
        {
            new() { Id = 7, Name = "Neo" }
        }.AsQueryable();

        var q = ProjectionHelpers.ProjectToInMemory<UserDto>(source, mapper);
        var list = q.ToList();

        list.Should().ContainSingle();

        list[0].Name.Should().Be("Neo");
    }

    [Fact]
    public void ProjectToInMemory_Typed_Works()
    {
        var mapper = new FakeMapper();
        var source = new List<User>
        {
            new() { Id = 3, Name = "Trinity" }
        }.AsQueryable();

        var q = ProjectionHelpers.ProjectToInMemory<User, UserDto>(source, mapper);

        q.Single().Id.Should().Be(3);
    }
}