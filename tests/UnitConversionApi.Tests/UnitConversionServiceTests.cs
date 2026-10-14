using UnitConversionApi.Conversions;
using UnitConversionApi.Exceptions;
using UnitConversionApi.Services;

namespace UnitConversionApi.Tests;

public class UnitConversionServiceTests
{
    private readonly UnitConversionService _service = new(
        new UnitRegistry(),
        new LengthConverter(),
        new MassConverter(),
        new TemperatureConverter());

    [Fact]
    public void MeterToFoot_ConvertsUsingMeterAsBase()
    {
        var response = _service.Convert(100d, "meter", "foot");

        Assert.Equal(100d, response.Value);
        Assert.Equal("meter", response.FromUnit);
        Assert.Equal("foot", response.ToUnit);
        Assert.Equal("Length", response.Category);
        Assert.Equal(100d / 0.3048d, response.Result, precision: 9);
    }

    [Fact]
    public void KilometerToMile_ConvertsUsingInternationalMile()
    {
        var response = _service.Convert(1d, "kilometer", "mile");

        Assert.Equal(1000d / 1609.344d, response.Result, precision: 9);
        Assert.Equal("Length", response.Category);
    }

    [Fact]
    public void CentimeterToInch_ConvertsUsingExactInchDefinition()
    {
        var response = _service.Convert(2.54d, "centimeter", "inch");

        Assert.Equal(1d, response.Result, precision: 9);
    }

    [Fact]
    public void KilogramToPound_ConvertsUsingAvoirdupoisPound()
    {
        var response = _service.Convert(1d, "kilogram", "pound");

        Assert.Equal(1d / 0.45359237d, response.Result, precision: 9);
        Assert.Equal("Mass", response.Category);
    }

    [Fact]
    public void GramToKilogram_ScalesByOneThousandth()
    {
        var response = _service.Convert(250d, "gram", "kilogram");

        Assert.Equal(0.25d, response.Result, precision: 9);
    }

    [Fact]
    public void CelsiusToFahrenheit_UsesAffineFormula()
    {
        var response = _service.Convert(100d, "celsius", "fahrenheit");

        Assert.Equal(212d, response.Result, precision: 9);
        Assert.Equal("Temperature", response.Category);
    }

    [Fact]
    public void FahrenheitToCelsius_UsesAffineFormula()
    {
        var response = _service.Convert(32d, "fahrenheit", "celsius");

        Assert.Equal(0d, response.Result, precision: 9);
    }

    [Fact]
    public void KelvinToCelsius_SubtractsAbsoluteZeroOffset()
    {
        var response = _service.Convert(273.15d, "kelvin", "celsius");

        Assert.Equal(0d, response.Result, precision: 9);
    }

    [Fact]
    public void NegativeCelsius_ConvertsToFahrenheit()
    {
        var response = _service.Convert(-40d, "celsius", "fahrenheit");

        Assert.Equal(-40d, response.Result, precision: 9);
    }

    [Fact]
    public void SameUnit_ReturnsOriginalValue()
    {
        var response = _service.Convert(12.5d, "yard", "yard");

        Assert.Equal(12.5d, response.Result);
        Assert.Equal("yard", response.FromUnit);
        Assert.Equal("yard", response.ToUnit);
    }

    [Fact]
    public void UnitNames_AreCaseInsensitive()
    {
        var response = _service.Convert(1d, "MeTeR", "FOOT");

        Assert.Equal("meter", response.FromUnit);
        Assert.Equal("foot", response.ToUnit);
        Assert.Equal(1d / 0.3048d, response.Result, precision: 9);
    }

    [Fact]
    public void UnknownSourceUnit_Throws()
    {
        var exception = Assert.Throws<UnitConversionException>(() => _service.Convert(1d, "furlong", "meter"));

        Assert.Contains("Unknown source unit", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownTargetUnit_Throws()
    {
        var exception = Assert.Throws<UnitConversionException>(() => _service.Convert(1d, "meter", "league"));

        Assert.Contains("Unknown target unit", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void IncompatibleCategories_Throw()
    {
        var exception = Assert.Throws<UnitConversionException>(() => _service.Convert(1d, "meter", "kilogram"));

        Assert.Contains("different categories", exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(-273.16d, "celsius")]
    [InlineData(-500d, "fahrenheit")]
    [InlineData(-1d, "kelvin")]
    public void TemperatureBelowAbsoluteZero_Throws(double value, string unit)
    {
        var exception = Assert.Throws<UnitConversionException>(() => _service.Convert(value, unit, "kelvin"));

        Assert.Contains("absolute zero", exception.Message, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(-273.15d, "celsius")]
    [InlineData(-459.67d, "fahrenheit")]
    [InlineData(0d, "kelvin")]
    public void TemperatureAtAbsoluteZero_IsAccepted(double value, string unit)
    {
        var response = _service.Convert(value, unit, "kelvin");

        Assert.InRange(response.Result, -1e-6d, 1e-6d);
    }

    [Fact]
    public void ZeroAndNegativeLinearValues_AreAccepted()
    {
        var zero = _service.Convert(0d, "pound", "ounce");
        var negative = _service.Convert(-2d, "foot", "inch");

        Assert.Equal(0d, zero.Result);
        Assert.Equal(-24d, negative.Result, precision: 9);
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void NonFiniteValues_Throw(double value)
    {
        var exception = Assert.Throws<UnitConversionException>(() => _service.Convert(value, "meter", "foot"));

        Assert.Contains("finite number", exception.Message, StringComparison.Ordinal);
    }
}
