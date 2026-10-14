namespace UnitConversionApi.Conversions;

/// <summary>
/// Converts length by scaling through meters.
/// </summary>
public sealed class LengthConverter
{
    /// <summary>
    /// Converts a length. The same unit returns the original value.
    /// </summary>
    public double Convert(double value, UnitDefinition from, UnitDefinition to)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        return LinearUnitMath.Convert(value, from, to);
    }
}
