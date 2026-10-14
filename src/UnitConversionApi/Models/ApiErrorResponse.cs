using Microsoft.AspNetCore.Mvc;

namespace UnitConversionApi.Models;

/// <summary>
/// Consistent error body for invalid requests and unexpected failures.
/// </summary>
public sealed class ApiErrorResponse
{
    /// <summary>
    /// URI identifying the error type.
    /// </summary>
    public string Type { get; init; } = "https://tools.ietf.org/html/rfc9110#section-15.5.1";

    /// <summary>
    /// Short, stable error title.
    /// </summary>
    public string Title { get; init; } = "Invalid request.";

    /// <summary>
    /// HTTP status code.
    /// </summary>
    public int Status { get; init; }

    /// <summary>
    /// Human-readable explanation safe to show to API clients.
    /// </summary>
    public string Detail { get; init; } = string.Empty;

    /// <summary>
    /// Creates a 400 response body.
    /// </summary>
    public static ApiErrorResponse BadRequest(string detail) => new()
    {
        Status = StatusCodes.Status400BadRequest,
        Detail = detail
    };
}

/// <summary>
/// Builds HTTP results that always use <see cref="ApiErrorResponse"/>.
/// </summary>
public static class ApiErrorResults
{
    /// <summary>
    /// Returns HTTP 400 with a problem-style JSON body.
    /// </summary>
    public static ObjectResult BadRequest(string detail)
    {
        return new ObjectResult(ApiErrorResponse.BadRequest(detail))
        {
            StatusCode = StatusCodes.Status400BadRequest,
            ContentTypes = { "application/problem+json" }
        };
    }
}
