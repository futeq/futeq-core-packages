using FluentAssertions;
using FQ.AspNetCore.Correlation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FQ.AspNetCore.Tests;

public class CorrelationIdMiddlewareTests
{
    private static TestServer BuildServer(Action<CorrelationOptions>? cfg = null)
    {
        return new TestServer(new WebHostBuilder()
            .ConfigureServices(s =>
            {
                s.AddOptions<CorrelationOptions>().Configure(o => cfg?.Invoke(o));
            })
            .Configure(app =>
            {
                var opts = app.ApplicationServices.GetRequiredService<IOptions<CorrelationOptions>>();
                app.UseMiddleware<CorrelationIdMiddleware>();
                app.Run(async ctx =>
                {
                    // echo header
                    var id = ctx.Response.Headers[opts.Value.HeaderName].ToString();
                    await ctx.Response.WriteAsync(string.IsNullOrEmpty(id) ? "no-id" : id);
                });
            }));
    }

    [Fact]
    public async Task Uses_Incoming_Header()
    {
        using var server = BuildServer(o => o.HeaderName = "X-Correlation-Id");
        var req = new HttpRequestMessage(HttpMethod.Get, "/");
        req.Headers.Add("X-Correlation-Id", "CID-abc");

        var resp = await server.CreateClient().SendAsync(req);
        var body = await resp.Content.ReadAsStringAsync();

        body.Should().Be("CID-abc");
        resp.Headers.Should().Contain(h => h.Key == "X-Correlation-Id");
    }

    [Fact]
    public async Task Generates_When_Missing()
    {
        using var server = BuildServer();
        var resp = await server.CreateClient().GetAsync("/");
        var body = await resp.Content.ReadAsStringAsync();

        body.Should().NotBe("no-id");
        resp.Headers.Should().Contain(h => h.Key == "X-Correlation-Id");
    }
}