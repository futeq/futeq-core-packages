using FluentAssertions;
using FQ.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

public class ResultProblemDetailsFactoryTests
{
    private static ProblemDetailsFactory CreateFactory()
    {
        var api = Options.Create(new ApiBehaviorOptions());
        return new ResultProblemDetailsFactory(api);
    }

    [Fact]
    public void CreateProblemDetails_Fills_Defaults()
    {
        var factory = CreateFactory();
        var pd = factory.CreateProblemDetails(httpContext: null, statusCode: 422, detail: "oops");

        pd.Status.Should().Be(422);
        pd.Title.Should().NotBeNullOrWhiteSpace();
        pd.Detail.Should().Be("oops");
        pd.Type.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void CreateValidationProblemDetails_Uses_Validation_Urn()
    {
        var factory = CreateFactory();

        var modelState = new ModelStateDictionary();
        modelState.AddModelError("name", "Required");

        var v = factory.CreateValidationProblemDetails(null!, modelState);

        v.Status.Should().Be(400);
        v.Type.Should().Contain("validation");
        v.Errors.Should().ContainKey("name");
    }
}