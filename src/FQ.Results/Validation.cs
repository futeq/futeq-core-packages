namespace FQ.Results;

/// <summary>
/// Validation helpers for aggregating field-level errors and extracting structured maps.
/// </summary>
public static class Validation
{
    /// <summary>
    /// Creates a <see cref="Error"/> of type <see cref="ErrorType.Validation"/> from a sequence
    /// of field failures, grouping messages by field.
    /// </summary>
    /// <param name="failures">Sequence of (Field, Message, Code) tuples.</param>
    /// <param name="title">Optional high-level message; defaults to "Validation failed".</param>
    /// <returns>An error with a structured "errors" map in <see cref="Error.Metadata"/>.</returns>
    public static Error FromFailures(IEnumerable<(string Field, string Message, string? Code)> failures, string title = "Validation failed")
    {
        var dict = failures
            .GroupBy(f => f.Field, f => f.Message)
            .ToDictionary(g => g.Key, g => g.ToArray());

        var meta = new Dictionary<string, object> { [Constants.ErrorKeyName] = dict };
        
        return Error.Validation(Constants.ValidationErrorCode, title, null, meta);
    }

    /// <summary>
    /// Attempts to extract the field-&gt;messages map from a validation <see cref="Error"/>.
    /// </summary>
    /// <param name="error">The error, expected to be of type <see cref="ErrorType.Validation"/>.</param>
    /// <returns>
    /// The structured error map if present; otherwise <c>null</c>.
    /// </returns>
    public static IReadOnlyDictionary<string, string[]>? TryExtractFieldMap(Error error)
    {
        if (error.Type != ErrorType.Validation || error.Metadata is null)
        {
            return null;
        }

        if (error.Metadata.TryGetValue(Constants.ErrorKeyName, out var obj) && obj is IReadOnlyDictionary<string, string[]> map)
        {
            return map;
        }

        if (error.Metadata.TryGetValue(Constants.ErrorKeyName, out var obj2) && obj2 is Dictionary<string, string[]> map2)
        {
            return map2;
        }

        return null;
    }
}