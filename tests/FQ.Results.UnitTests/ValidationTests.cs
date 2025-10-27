using FluentAssertions;

namespace FQ.Results.UnitTests;

public class ValidationTests
{
    [Fact]
    public void FromFailures_Groups_By_Field()
    {
        var err = Validation.FromFailures(new[]
        {
            ("name","Required",(string?)"req"),
            ("name","Too short",null),
            ("email","Invalid",null)
        });

        err.Type.Should().Be(ErrorType.Validation);
        
        var map = Validation.TryExtractFieldMap(err)!;
        
        map.Should().ContainKey("name");
        map["name"].Should().BeEquivalentTo(new[] { "Required", "Too short" });
        map["email"].Should().ContainSingle("Invalid");
    }

    [Fact]
    public void TryExtractFieldMap_Returns_Null_For_NonValidation_Error()
    {
        var e = Error.NotFound("x","y");
        
        Validation.TryExtractFieldMap(e).Should().BeNull();
    }
}