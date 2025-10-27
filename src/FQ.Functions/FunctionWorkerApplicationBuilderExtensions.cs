using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;

namespace FQ.Functions;

public static class FunctionWorkerApplicationBuilderExtensions
{
    public static IFunctionsWorkerApplicationBuilder UseFunctionsUtilities(
        this IFunctionsWorkerApplicationBuilder builder)
    {
        builder.UseMiddleware<CorrelationMiddleware>();
        builder.UseMiddleware<IdempotencyMiddleware>();   
        builder.UseMiddleware<FunctionExceptionMiddleware>();

        return builder;
    }
}