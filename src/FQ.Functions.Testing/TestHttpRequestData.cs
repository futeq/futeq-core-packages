using System.Security.Claims;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace FQ.Functions.Testing;

public sealed class TestHttpRequestData : HttpRequestData
{
    public TestHttpRequestData(FunctionContext context, Uri? url = null) : base(context)
    {
        Url = url ?? new Uri("https://localhost/api/test");
        Headers = new HttpHeadersCollection();
        Body = new MemoryStream();
        Method = "GET";
    }

    public override HttpResponseData CreateResponse() => new TestHttpResponseData(FunctionTestingHelper.Ctx());

    public override Stream Body { get;  }
    public override HttpHeadersCollection Headers { get; }
    public override IReadOnlyCollection<IHttpCookie> Cookies { get; } = [];
    public override Uri Url { get; }
    public override IEnumerable<ClaimsIdentity> Identities { get; } = [];
    public override string Method { get; }
}