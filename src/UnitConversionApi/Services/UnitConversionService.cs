using UnitConversionApi.Conversions;
using UnitConversionApi.Exceptions;
using UnitConversionApi.Models;

namespace UnitConversionApi.Services;

/// <summary>
/// Routes a request to the length, mass, or temperature converter.
/// Calculations use <see cref="double"/> (IEEE-754 binary64) and do not round intermediate results.
/// </summary>
public sealed class UnitConversionService : IUnitConversionService
{
    private readonly UnitRegistry _registry;
    private readonly LengthConverter _lengthConverter;
    private readonly MassConverter _massConverter;
    private readonly TemperatureConverter _temperatureConverter;

    /// <summary>
    /// Creates the conversion service.
    /// </summary>
    public UnitConversionService(
        UnitRegistry registry,
        LengthConverter lengthConverter,
        MassConverter massConverter,
        TemperatureConverter temperatureConverter)
    {
        _registry = registry;
        _lengthConverter = lengthConverter;
        _massConverter = massConverter;
        _temperatureConverter = temperatureConverter;
    }

    /// <inheritdoc />
    public ConversionResponse Convert(double value, string fromUnit, string toUnit)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new UnitConversionException("Value must be a finite number. NaN and infinity are not supported.");
        }

        if (string.IsNullOrWhiteSpace(fromUnit))
        {
            throw new UnitConversionException("Source unit is required.");
        }

        if (string.IsNullOrWhiteSpace(toUnit))
        {
            throw new UnitConversionException("Target unit is required.");
        }

        if (!_registry.TryGet(fromUnit, out var from) || from is null)
        {
            throw new UnitConversionException($"Unknown source unit '{fromUnit.Trim()}'.");
        }

        if (!_registry.TryGet(toUnit, out var to) || to is null)
        {
            throw new UnitConversionException($"Unknown target unit '{toUnit.Trim()}'.");
        }

        if (from.Category != to.Category)
        {
            throw new UnitConversionException(
                $"Cannot convert '{from.Name}' ({from.Category}) to '{to.Name}' ({to.Category}) because the units belong to different categories.");
        }

        var result = from.Category switch
        {
            UnitCategory.Length => _lengthConverter.Convert(value, from, to),
            UnitCategory.Mass => _massConverter.Convert(value, from, to),
            UnitCategory.Temperature => _temperatureConverter.Convert(value, from, to),
            _ => throw new UnitConversionException($"Category '{from.Category}' is not supported.")
        };

        return new ConversionResponse
        {
            Value = value,
            FromUnit = from.Name,
            ToUnit = to.Name,
            Result = result,
            Category = from.Category.ToString()
        };
    }
}
