using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

/// <summary>
/// Converts a numeric value between two units in the same category.
/// </summary>
public interface IUnitConversionService
{
    /// <summary>
    /// Converts <paramref name="value"/> from <paramref name="fromUnit"/> to <paramref name="toUnit"/>.
    /// </summary>
    /// <exception cref="Exceptions.UnitConversionException">
    /// The value is not finite, a unit is unknown, the categories differ, or a temperature is below absolute zero.
    /// </exception>
    ConversionResponse Convert(double value, string fromUnit, string toUnit);
}
