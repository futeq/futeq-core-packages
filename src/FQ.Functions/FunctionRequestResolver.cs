// in FQ.Web.Functions

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace FQ.Functions;

// a tiny delegate so we can inject a fake in tests
public delegate Task<HttpRequestData?> FunctionRequestResolver(FunctionContext ctx);