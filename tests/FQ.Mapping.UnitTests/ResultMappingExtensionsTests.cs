using FluentAssertions;
using FQ.Results;

namespace FQ.Mapping.UnitTests;

public class ResultMappingExtensionsTests
{
    private readonly IObjectMapper _mapper = new FakeMapper();

    [Fact]
    public void MapResult_Should_Map_On_Success_And_Preserve_Error()
    {
        Result<User> ok = new User { Id = 1, Name = "Ada" };
        var r1 = _mapper.MapResult<User, UserDto>(ok);
        r1.IsSuccess.Should().BeTrue();
        r1.Value!.Name.Should().Be("Ada");

        Result<User> fail = Error.NotFound("user_nf","User not found");
        var r2 = _mapper.MapResult<User, UserDto>(fail);
        r2.IsSuccess.Should().BeFalse();
        r2.Error!.Code.Should().Be("user_nf");
    }

    [Fact]
    public async Task MapResultAsync_Should_Map_On_Success_And_Preserve_Error()
    {
        Task<Result<User>> okTask = Task.FromResult<Result<User>>(new User { Id = 2, Name = "Linus" });
        var r1 = await _mapper.MapResultAsync<User, UserDto>(okTask);
        r1.IsSuccess.Should().BeTrue();
        r1.Value!.Id.Should().Be(2);

        Task<Result<User>> failTask = Task.FromResult(Result<User>.Fail(Error.Conflict("c","x")));
        var r2 = await _mapper.MapResultAsync<User, UserDto>(failTask);
        r2.IsSuccess.Should().BeFalse();
        r2.Error!.Type.Should().Be(ErrorType.Conflict);
    }
}