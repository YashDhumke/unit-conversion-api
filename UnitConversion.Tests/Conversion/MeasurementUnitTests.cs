using UnitConversion.Domain.Models.Conversion;

namespace UnitConversion.Tests.Conversion;

public class MeasurementUnitTests
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    [InlineData(100, 100)]
    public void ConvertToBase_BaseUnit_ReturnsSameValue(decimal input, decimal expected)
    {
        var unit = new MeasurementUnit("m", "Meter", "m", "length", 1m);
        Assert.Equal(expected, unit.ConvertToBase(input));
    }

    [Theory]
    [InlineData(1, 1000)]
    [InlineData(0.5, 500)]
    public void ConvertToBase_Kilometer_ReturnsMeters(decimal km, decimal expectedMeters)
    {
        var km_unit = new MeasurementUnit("km", "Kilometer", "km", "length", 1000m);
        Assert.Equal(expectedMeters, km_unit.ConvertToBase(km));
    }

    [Theory]
    [InlineData(1000, 1)]
    [InlineData(500, 0.5)]
    public void ConvertFromBase_Kilometer_ReturnsKilometers(decimal meters, decimal expectedKm)
    {
        var km_unit = new MeasurementUnit("km", "Kilometer", "km", "length", 1000m);
        Assert.Equal(expectedKm, km_unit.ConvertFromBase(meters));
    }

    [Fact]
    public void ConvertToBase_Fahrenheit_ConvertsCorrectly()
    {
        // 32°F → 0°C: (32 + (-32)) * 5/9 = 0
        var fahrenheit = new MeasurementUnit("f", "Fahrenheit", "degF", "temperature", 5m / 9m, -32m);
        var result = fahrenheit.ConvertToBase(32m);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void ConvertToBase_Kelvin_ConvertsCorrectly()
    {
        // 273.15K → 0°C: (273.15 + (-273.15)) * 1 = 0
        var kelvin = new MeasurementUnit("k", "Kelvin", "K", "temperature", 1m, -273.15m);
        var result = kelvin.ConvertToBase(273.15m);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void ConvertFromBase_Fahrenheit_ConvertsCorrectly()
    {
        // 0°C → 32°F: (0 / (5/9)) - (-32) = 32
        var fahrenheit = new MeasurementUnit("f", "Fahrenheit", "degF", "temperature", 5m / 9m, -32m);
        var result = fahrenheit.ConvertFromBase(0m);
        Assert.Equal(32m, result);
    }

    [Fact]
    public void ConvertFromBase_Kelvin_ConvertsCorrectly()
    {
        // 0°C → 273.15K: (0 / 1) - (-273.15) = 273.15
        var kelvin = new MeasurementUnit("k", "Kelvin", "K", "temperature", 1m, -273.15m);
        var result = kelvin.ConvertFromBase(0m);
        Assert.Equal(273.15m, result);
    }
}
