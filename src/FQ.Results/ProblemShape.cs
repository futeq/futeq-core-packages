namespace FQ.Results;

/// <summary>
/// Minimal RFC 7807-compatible structure.
/// Keep this API-agnostic and map to ASP.NET's ProblemDetails at the edge.
/// </summary>
/// <param name="Title">Short, human-readable summary of the problem type.</param>
/// <param name="Status">HTTP status code applicable to this occurrence of the problem.</param>
/// <param name="Type">A URI reference that identifies the problem type.</param>
/// <param name="Detail">Human-readable explanation specific to this occurrence of the problem.</param>
/// <param name="Instance">A URI reference that identifies the specific occurrence of the problem.</param>
/// <param name="Errors">
/// Optional map of field names to error messages (primarily for validation).
/// </param>
public sealed record ProblemShape(
    string Title,
    int Status,
    string Type,
    string Detail,
    string? Instance = null,
    IReadOnlyDictionary<string, string[]>? Errors = null);