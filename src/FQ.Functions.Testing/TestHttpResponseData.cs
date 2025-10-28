using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace FQ.Functions.Testing;

public sealed class TestHttpResponseData : HttpResponseData
{
    public TestHttpResponseData(FunctionContext functionContext) : base(functionContext)
    {
        Cookies = new TestHttpCookies();
        Headers = new HttpHeadersCollection();
        Body = new MemoryStream();
    }

    public override HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    public override HttpHeadersCollection Headers { get; set; }
    public override Stream Body { get; set; }
    public override HttpCookies Cookies { get; } 
    
    private sealed class TestHttpCookies : HttpCookies
    {
        private readonly Dictionary<string, IHttpCookie> _cookies = new(StringComparer.OrdinalIgnoreCase);

        public override void Append(string name, string value)
        {
            _cookies[name] = new TestHttpCookie(name, value);
        }

        public override void Append(IHttpCookie cookie)
        {
            throw new NotImplementedException();
        }

        public override IHttpCookie CreateNew()
        {
            return new TestHttpCookie();
        }
    }

    private sealed class TestHttpCookie : IHttpCookie
    {
        public string? Domain { get; set; }
        public DateTimeOffset? Expires { get; set; }
        public bool? HttpOnly { get; set; }
        public double? MaxAge { get; set; }
        public string Name { get; }
        public string? Path { get; set; }
        public SameSite SameSite { get; set; }
        public bool? Secure { get; set; }
        public string Value { get; }

        public TestHttpCookie(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public TestHttpCookie() : this(string.Empty, string.Empty)
        {
        }
    }
}