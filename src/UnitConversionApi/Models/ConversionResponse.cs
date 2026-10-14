namespace UnitConversionApi.Models;

/// <summary>
/// Successful conversion result.
/// </summary>
public sealed class ConversionResponse
{
    /// <summary>
    /// The value that was converted.
    /// </summary>
    public double Value { get; init; }

    /// <summary>
    /// Canonical source unit name.
    /// </summary>
    public string FromUnit { get; init; } = string.Empty;

    /// <summary>
    /// Canonical target unit name.
    /// </summary>
    public string ToUnit { get; init; } = string.Empty;

    /// <summary>
    /// Converted value. Intermediate calculations are not rounded.
    /// </summary>
    public double Result { get; init; }

    /// <summary>
    /// Measurement category shared by the source and target units.
    /// </summary>
    public string Category { get; init; } = string.Empty;
}
