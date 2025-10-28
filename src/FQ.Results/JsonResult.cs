namespace FQ.Results;

internal struct JsonResult
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Gets the error when <see cref="IsSuccess"/> is <c>false</c>; otherwise <c>null</c>.
    /// </summary>
    public Error? Error { get; set; }
}

internal record struct JsonResult<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; set;  }

    /// <summary>
    /// Gets the value when <see cref="IsSuccess"/> is <c>true</c>; otherwise <c>null</c>.
    /// </summary>
    public T? Value { get; set; }

    /// <summary>
    /// Gets the error when <see cref="IsSuccess"/> is <c>false</c>; otherwise <c>null</c>.
    /// </summary>
    public Error? Error { get; set; }
}