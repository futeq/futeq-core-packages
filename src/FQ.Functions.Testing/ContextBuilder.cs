using Microsoft.Azure.Functions.Worker;
using NSubstitute;

namespace FQ.Functions.Testing;

internal static class ContextBuilder
{
    private sealed class TestInvocationResult : InvocationResult { public TestInvocationResult() : base() { }
        public override object? Value { get; set; }
    }

    public static FunctionContext New()
    {
        var ctx = Substitute.For<FunctionContext>();
        ctx.Items.Returns(new Dictionary<object, object>());
        ctx.GetInvocationResult().Returns(new TestInvocationResult());
        return ctx;
    }
}