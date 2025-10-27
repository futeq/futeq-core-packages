using FluentAssertions;
using FQ.Results;
using Mapster;

namespace FQ.Mapping.Mapster.UnitTests;

internal sealed class User
{
    public int Id { get; set; }
    public string GivenName { get; set; } = "";
    public string FamilyName { get; set; } = "";
}
internal sealed class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
}

public class MapsterObjectMapperTests
{
    private static MapsterObjectMapper CreateMapper()
    {
        var cfg = new TypeAdapterConfig();
        // Projection: FullName = GivenName + " " + FamilyName
        cfg.NewConfig<User, UserDto>()
            .Map(d => d.FullName, s => s.GivenName + " " + s.FamilyName);
        return new MapsterObjectMapper(cfg);
    }

    [Fact]
    public void Map_Should_Use_Config()
    {
        var mapper = CreateMapper();
        var dto = mapper.Map<User, UserDto>(new User { Id = 1, GivenName = "Ada", FamilyName = "Lovelace" });

        dto.Id.Should().Be(1);
        dto.FullName.Should().Be("Ada Lovelace");
    }

    [Fact]
    public void ProjectTo_Should_Project_Over_IQueryable()
    {
        var mapper = CreateMapper();
        var data = new[]
        {
            new User{ Id=1, GivenName="Alan", FamilyName="Turing" },
            new User{ Id=2, GivenName="Grace", FamilyName="Hopper" }
        }.AsQueryable();

        // This uses Mapster's expression projection, even over LINQ-to-Objects here
        var q = mapper.ProjectTo<User, UserDto>(data);
        var list = q.ToList();

        list.Select(x => x.FullName).Should().Equal("Alan Turing", "Grace Hopper");
    }

    [Fact]
    public void MapResult_Should_Work_With_Adapter()
    {
        var mapper = CreateMapper();
        Result<User> ok = new User { Id = 9, GivenName = "Barbara", FamilyName = "Liskov" };

        var r = mapper.MapResult<User, UserDto>(ok);

        r.IsSuccess.Should().BeTrue();
        r.Value!.FullName.Should().Be("Barbara Liskov");
    }

    [Fact]
    public async Task MapResultAsync_Should_Work_With_Adapter()
    {
        var mapper = CreateMapper();
        Task<Result<User>> okTask = Task.FromResult<Result<User>>(
            new User { Id = 11, GivenName = "Bjarne", FamilyName = "Stroustrup" });

        var r = await mapper.MapResultAsync<User, UserDto>(okTask);

        r.IsSuccess.Should().BeTrue();
        r.Value!.FullName.Should().Be("Bjarne Stroustrup");
    }
}