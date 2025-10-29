using Microsoft.Azure.Functions.Worker;

namespace FQ.Functions;

public interface IFunctionContextAccessor
{
    FunctionContext? Current { get; set; }
}