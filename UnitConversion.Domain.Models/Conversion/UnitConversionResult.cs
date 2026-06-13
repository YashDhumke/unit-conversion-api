namespace UnitConversion.Domain.Models.Conversion;

public class UnitConversionResult
{
    public string Category { get; init; } = string.Empty;
    public MeasurementUnit FromUnit { get; init; } = null!;
    public MeasurementUnit ToUnit { get; init; } = null!;
    public decimal InputValue { get; init; }
    public decimal ConvertedValue { get; init; }
}
