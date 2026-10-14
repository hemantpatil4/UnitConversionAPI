namespace UnitConversionApi.Conversions;

/// <summary>
/// Converts mass by scaling through kilograms.
/// </summary>
public sealed class MassConverter
{
    /// <summary>
    /// Converts a mass. The same unit returns the original value.
    /// </summary>
    public double Convert(double value, UnitDefinition from, UnitDefinition to)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        return LinearUnitMath.Convert(value, from, to);
    }
}
