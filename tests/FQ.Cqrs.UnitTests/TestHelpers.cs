using System.Text.Json;
using MediatR;

namespace FQ.Cqrs.UnitTests;

public static class TestHelpers
{
    public static RequestHandlerDelegate<FQ.Results.Result> NextOk()
        => (CancellationToken _) => Task.FromResult<FQ.Results.Result>(FQ.Results.Result.Ok());

    public static RequestHandlerDelegate<FQ.Results.Result> NextFail(FQ.Results.Error? e = null)
        => (CancellationToken _) => Task.FromResult<FQ.Results.Result>(
            FQ.Results.Result.Fail(e ?? FQ.Results.Error.Validation("bad", "nope"))
        );

    public static RequestHandlerDelegate<FQ.Results.Result<T>> NextOk<T>(T value)
        => (CancellationToken _) => Task.FromResult<FQ.Results.Result<T>>(FQ.Results.Result<T>.Ok(value));

    public static RequestHandlerDelegate<FQ.Results.Result<T>> NextFail<T>(FQ.Results.Error? e = null)
        => (CancellationToken _) => Task.FromResult<FQ.Results.Result<T>>(
            FQ.Results.Result<T>.Fail(e ?? FQ.Results.Error.Validation("bad", "nope"))
        );
    
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}