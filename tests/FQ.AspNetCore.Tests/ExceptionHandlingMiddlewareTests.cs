using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace FQ.AspNetCore.Tests;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task Unhandled_Exception_Returns_Rfc7807()
    {
        var builder = new WebHostBuilder()
            .Configure(app =>
            {
                app.UseMiddleware<ExceptionHandlingMiddleware>();
                app.Run(_ => throw new InvalidOperationException("boom"));
            });

        using var server = new TestServer(builder);
        var resp = await server.CreateClient().GetAsync("/x");

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest); // mapping from Error.FromException default
        resp.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");

        var json = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.TryGetProperty("title", out _).Should().BeTrue();
        doc.RootElement.TryGetProperty("status", out _).Should().BeTrue();
        doc.RootElement.TryGetProperty("detail", out _).Should().BeTrue();
    }
}