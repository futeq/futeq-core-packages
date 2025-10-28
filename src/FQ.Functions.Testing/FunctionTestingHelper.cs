using System.Net;
using Microsoft.Azure.Functions.Worker;

namespace FQ.Functions.Testing;

public static class FunctionTestingHelper
{
    public static FunctionContext Ctx() => ContextBuilder.New();

    public static (TestHttpRequestData req, TestHttpResponseData res) Http(FunctionContext ctx)
    {
        var req = new TestHttpRequestData(ctx);
        var res = new TestHttpResponseData(ctx) { StatusCode = HttpStatusCode.OK };
        return (req, res);
    }
}