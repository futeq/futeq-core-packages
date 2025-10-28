using System.Text.Json;
using FluentAssertions;

namespace FQ.Results.UnitTests;

public class SerializationTests
{
    private static readonly JsonSerializerOptions Opts = new(JsonSerializerDefaults.Web);

    [Fact]
    public void Can_Serialize_And_Deserialize_Result()
    {
        var r = Result.Fail(Error.NotFound("order_not_found","Order missing"));
        var json = JsonSerializer.Serialize(r, Opts);
        var restored = JsonResultSerializer.Deserialize(json, Opts);
 
        restored!.IsSuccess.Should().BeFalse();
        restored.Error!.Code.Should().Be("order_not_found");
    }

    [Fact]
    public void Can_Serialize_And_Deserialize_ResultT()
    {
        var r = Result<int>.Ok(42);
        var json = JsonSerializer.Serialize(r, Opts);
        var restored = JsonResultSerializer.Deserialize<int>(json, Opts);
 
        restored!.IsSuccess.Should().BeTrue();
        restored.Value.Should().Be(42);
    }

    [Fact]
    public void Can_Serialize_Validation_Error_With_FieldMap()
    {
        var e = Validation.FromFailures([("name","Required",(string?)null)]);
        var json = JsonSerializer.Serialize(e, Opts);
        var restored = JsonSerializer.Deserialize<Error>(json, Opts)!;

        restored.Type.Should().Be(ErrorType.Validation);
        
        var map = Validation.TryExtractFieldMap(restored)!;
        
        map.Should().BeNull();
    }
}