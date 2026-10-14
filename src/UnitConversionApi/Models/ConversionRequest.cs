namespace UnitConversionApi.Models;

/// <summary>
/// Query parameters for a unit conversion.
/// </summary>
public sealed class ConversionRequest
{
    /// <summary>
    /// The finite numeric value to convert. Negative values are accepted when the unit allows them.
    /// </summary>
    public double? Value { get; init; }

    /// <summary>
    /// Case-insensitive source unit name, such as <c>meter</c> or <c>celsius</c>.
    /// </summary>
    public string? From { get; init; }

    /// <summary>
    /// Case-insensitive target unit name, such as <c>foot</c> or <c>fahrenheit</c>.
    /// </summary>
    public string? To { get; init; }
}
