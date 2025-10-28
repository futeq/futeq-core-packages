# Futeq Core Packages

A modern, minimal, and production-ready foundation for building **.NET 9** services and APIs —  
designed around **Clean Architecture**, **CQRS**, and **unified result handling**.

This ecosystem includes:

| Package | Purpose |
|----------|----------|
| **FQ.Results** | Uniform success/error result handling, HTTP mapping, and problem-details integration |
| **FQ.Cqrs** | MediatR pipeline behaviors for CQRS (validation, authorization, idempotency, logging, performance) |
| **FQ.AspNetCore** | ASP.NET Core utilities (correlation, idempotency, versioning, result mapping) |
| **FQ.Functions** | Azure Functions utilities (correlation, idempotency, exception handling, result writers) |
| **FQ.Mapping** | Tiny abstraction over object mapping with first‑class `Result<T>` support |

All libraries target **.NET 9** and work with:
- **MediatR 13.1+**
- **Microsoft.Azure.Functions.Worker 2.2+**
- **ASP.NET Core 9.0+**

---

## 📦 Installation

```bash
dotnet add package FQ.Results
dotnet add package FQ.Cqrs
dotnet add package FQ.AspNetCore
dotnet add package FQ.Functions
dotnet add package FQ.Mapping
```

---

## 🧩 FQ.Results

Provides lightweight primitives for handling outcomes and errors across your entire stack.

### Result basics

```csharp
using FQ.Results;

var ok = Result.Ok();
var user = Result<User>.Ok(new User("alice"));
var notFound = Result.Fail(Error.NotFound("user_not_found", "User does not exist"));
```

### Error factory helpers

```csharp
var e1 = Error.Validation("email", "Invalid format");
var e2 = Error.Forbidden("no_access", "User has no permission");
var e3 = Error.Conflict("duplicate", "Email already exists");
```

### Mapping to HTTP Problem Details

```csharp
var shape = e1.ToProblemShape("/api/users/1");

Console.WriteLine(shape.Status);  // 400
Console.WriteLine(shape.Type);    // urn:problem-type:validation
Console.WriteLine(shape.Detail);  // "Invalid format"
```

### JSON-friendly model

All result and error types are fully serializable with `System.Text.Json`.

```csharp
var json = JsonSerializer.Serialize(Result.Fail(Error.NotFound("x", "missing")));
```

Due to immutability of the record classes, use `FQ.Results.JsonResultSerializer` for deserialization purposes.

```csharp
var result = JsonResultSerializer.Deserialize<ResultType>(json, _serializerOptions);
```

---

## ⚙️ FQ.Cqrs

Integrates clean CQRS patterns using **MediatR pipeline behaviors** and **Result** semantics.

### CQRS Setup

In your `Startup` or DI registration:

```csharp
services.AddCqrsUtilities()
```

### Handlers

```csharp
public sealed record CreateUser(string Email, string Password) : ICommand<Result<Guid>>;

public sealed class CreateUserHandler : IRequestHandler<CreateUser, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateUser cmd, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cmd.Email))
            return Result<Guid>.Fail(Error.Validation("email", "Email is required"));

        var id = Guid.NewGuid();
        return Result<Guid>.Ok(id);
    }
}
```

### Authorizers

```csharp
public sealed class CreateUserAuthorizer : IAuthorizer<CreateUser>
{
    public Task<Result> AuthorizeAsync(CreateUser request, CancellationToken ct)
    {
        if (request.Email.EndsWith("@futeq.com"))
            return Task.FromResult(Result.Ok());

        return Task.FromResult(Result.Fail(Error.Forbidden("domain_not_allowed")));
    }
}
```

### Idempotent requests

```csharp
public sealed record CreateOrder(Guid Id, string Customer)
    : ICommand<Result<Guid>>, IIdempotentRequest
{
    public TimeSpan IdempotencyTtl => TimeSpan.FromMinutes(5);
}
```

---

## 🌐 FQ.AspNetCore

Utilities for ASP.NET Core 9.0 projects — standardized middleware and extensions.

### Register services

```csharp
builder.Services.AddAspNetCoreUtilities();
app.UseAspNetCoreUtilities();
```

### Controller helpers

```csharp
[HttpPost("users")]
public IActionResult Create([FromBody] CreateUser command)
{
    var result = _mediator.Send(command).Result;
    return result.ToActionResult(this);
}
```

---

## ☁️ FQ.Functions

Azure Functions isolated worker helpers.

### Configure

```csharp
var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        builder.UseFunctionsUtilities();
    })
    .ConfigureServices(services =>
    {
        services.AddFunctionsUtilities();
    })
    .Build();

host.Run();
```

### Usage

```csharp
public class UsersFunctions
{
    [Function("GetUser")]
    public async Task<HttpResponseData> GetUser(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/{id}")] HttpRequestData req, string id)
    {
        if (id == "404")
        {
                return await req.WriteResultAsync(Result.Fail(Error.NotFound("user", "Not found")));
        }

        return await req.WriteResultAsync(Result.Ok());
    }
}
```

---

## ✅ Testing

All packages have tests (xUnit + FluentAssertions + NSubstitute).

```bash
dotnet test -c Release
```

---

## 🧩 License

MIT © 2025 Futeq
