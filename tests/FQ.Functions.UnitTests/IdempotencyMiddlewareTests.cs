using FluentAssertions;
using FQ.Functions.Testing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;

namespace FQ.Functions.UnitTests;

public class IdempotencyMiddlewareTests
{ 
    [Fact]
    public async Task Captures_Idempotency_Key()
    {
        var ctx = FunctionTestingHelper.Ctx();
        var (req, _) = FunctionTestingHelper.Http(ctx);
        
        req.Headers.Add("Idempotency-Key", "IDEMP-1");

        var mw = new IdempotencyMiddleware(
            Options.Create(new IdempotencyOptions { HeaderName = "Idempotency-Key" }),
            resolve: _ => Task.FromResult<HttpRequestData?>(req));

        await mw.Invoke(ctx, _ => Task.CompletedTask);

        var accessor = new HttpIdempotencyKeyAccessor(new FakeContextAccessor(ctx));
        
        accessor.GetKey().Should().Be("IDEMP-1");
    }

    private class FakeContextAccessor : IFunctionContextAccessor
    {
        public FunctionContext? Current { get; set; }
        
        public FakeContextAccessor(FunctionContext context)
        {
            Current = context;
        } 
    }
}