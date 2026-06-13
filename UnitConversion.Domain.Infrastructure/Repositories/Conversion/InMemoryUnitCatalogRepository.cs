using UnitConversion.Domain.Core.Repositories.Conversion;
using UnitConversion.Domain.Models.Conversion;

namespace UnitConversion.Domain.Infrastructure.Repositories.Conversion;

public class InMemoryUnitCatalogRepository : IUnitCatalogRepository
{
    private static readonly IReadOnlyCollection<MeasurementUnit> UnitList =
    [
        // Length (Base Unit: Meter)
        new("m", "Meter", "m", ConversionCategory.Length, 1m),
        new("km", "Kilometer", "km", ConversionCategory.Length, 1000m),
        new("cm", "Centimeter", "cm", ConversionCategory.Length, 0.01m),
        new("mm", "Millimeter", "mm", ConversionCategory.Length, 0.001m),
        new("ft", "Foot", "ft", ConversionCategory.Length, 0.3048m),
        new("in", "Inch", "in", ConversionCategory.Length, 0.0254m),
        new("yd", "Yard", "yd", ConversionCategory.Length, 0.9144m),
        new("mi", "Mile", "mi", ConversionCategory.Length, 1609.344m),

        // Weight/Mass (Base Unit: Kilogram)
        new("kg", "Kilogram", "kg", ConversionCategory.Weight, 1m),
        new("g", "Gram", "g", ConversionCategory.Weight, 0.001m),
        new("mg", "Milligram", "mg", ConversionCategory.Weight, 0.000001m),
        new("lb", "Pound", "lb", ConversionCategory.Weight, 0.45359237m),
        new("oz", "Ounce", "oz", ConversionCategory.Weight, 0.028349523125m),

        // Temperature (Base Unit: Celsius)
        new("c", "Celsius", "degC", ConversionCategory.Temperature, 1m),
        new("f", "Fahrenheit", "degF", ConversionCategory.Temperature, 5m / 9m, -32m),
        new("k", "Kelvin", "K", ConversionCategory.Temperature, 1m, -273.15m),

        // Volume (Base = Liter)
        new("l", "Liter", "L", ConversionCategory.Volume, 1m),
        new("ml", "Milliliter", "mL", ConversionCategory.Volume, 0.001m),
        new("cl", "Centiliter", "cL", ConversionCategory.Volume, 0.01m),
        new("m3", "Cubic Meter", "m�", ConversionCategory.Volume, 1000m),
        new("gal", "Gallon", "gal", ConversionCategory.Volume, 3.78541m),

        // Time (Base = Second)
        new("s", "Second", "s", ConversionCategory.Time, 1m),
        new("min", "Minute", "min", ConversionCategory.Time, 60m),
        new("hr", "Hour", "hr", ConversionCategory.Time, 3600m),
        new("day", "Day", "day", ConversionCategory.Time, 86400m),

        // Speed (Base = Meters per Second)
        new("mps", "Meters per Second", "m/s", ConversionCategory.Speed, 1m),
        new("kph", "Kilometers per Hour", "km/h", ConversionCategory.Speed, 1m / 3.6m),
        new("mph", "Miles per Hour", "mph", ConversionCategory.Speed, 0.44704m),
        new("kn", "Knot", "kn", ConversionCategory.Speed, 0.514444m),
    ];

    public Task<IReadOnlyCollection<MeasurementUnit>> GetAllAsync()
    {
        return Task.FromResult(UnitList);
    }

    public Task<MeasurementUnit?> GetByCodeAsync(string code)
    {
        var unit = UnitList.FirstOrDefault(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(unit);
    }
}
