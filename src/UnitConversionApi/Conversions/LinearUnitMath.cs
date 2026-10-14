namespace UnitConversionApi.Conversions;

/// <summary>
/// Shared scale conversion for units that differ by a constant factor.
/// </summary>
internal static class LinearUnitMath
{
    public static double Convert(double value, UnitDefinition from, UnitDefinition to)
    {
        if (from.Name.Equals(to.Name, StringComparison.Ordinal))
        {
            return value;
        }

        return value * from.FactorToBase / to.FactorToBase;
    }
}
