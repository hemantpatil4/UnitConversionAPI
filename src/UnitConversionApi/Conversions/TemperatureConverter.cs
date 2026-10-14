using UnitConversionApi.Exceptions;

namespace UnitConversionApi.Conversions;

/// <summary>
/// Converts temperature through Celsius. Temperature is an affine scale, so it is not a multiplication factor.
/// </summary>
public sealed class TemperatureConverter
{
    private const double AbsoluteZeroCelsius = -273.15d;

    /// <summary>
    /// Values this close to absolute zero, in kelvin, are treated as sitting on the boundary.
    /// This absorbs binary floating-point error in literals such as -459.67.
    /// </summary>
    private const double AbsoluteZeroToleranceKelvin = 1e-6d;

    /// <summary>
    /// Converts a temperature. Values below absolute zero are rejected.
    /// </summary>
    public double Convert(double value, UnitDefinition from, UnitDefinition to)
    {
        ArgumentNullException.ThrowIfNull(from);
        ArgumentNullException.ThrowIfNull(to);
        EnsureNotBelowAbsoluteZero(value, from.Name);

        if (from.Name.Equals(to.Name, StringComparison.Ordinal))
        {
            return value;
        }

        var celsius = ToCelsius(value, from.Name);
        return FromCelsius(celsius, to.Name);
    }

    private static void EnsureNotBelowAbsoluteZero(double value, string unitName)
    {
        var kelvin = ToKelvin(value, unitName);
        if (kelvin < -AbsoluteZeroToleranceKelvin)
        {
            throw new UnitConversionException(
                $"Temperature {Format(value)} {unitName} is below absolute zero.");
        }
    }

    private static double ToCelsius(double value, string unitName) => unitName switch
    {
        "celsius" => value,
        "fahrenheit" => (value - 32d) * 5d / 9d,
        "kelvin" => value + AbsoluteZeroCelsius,
        _ => throw new UnitConversionException($"Unknown temperature unit '{unitName}'.")
    };

    private static double FromCelsius(double celsius, string unitName) => unitName switch
    {
        "celsius" => celsius,
        "fahrenheit" => (celsius * 9d / 5d) + 32d,
        "kelvin" => celsius - AbsoluteZeroCelsius,
        _ => throw new UnitConversionException($"Unknown temperature unit '{unitName}'.")
    };

    private static double ToKelvin(double value, string unitName) =>
        FromCelsius(ToCelsius(value, unitName), "kelvin");

    private static string Format(double value) => value.ToString("G17", System.Globalization.CultureInfo.InvariantCulture);
}
