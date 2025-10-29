using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace FQ.Functions.Accessors;

public sealed class FunctionContextAccessorMiddleware : IFunctionsWorkerMiddleware
{
    private readonly IFunctionContextAccessor _accessor;

    public FunctionContextAccessorMiddleware(IFunctionContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        _accessor.Current = context;

        try
        {
            await next(context);
        }
        finally
        {
            _accessor.Current = null;
        }
    }
}