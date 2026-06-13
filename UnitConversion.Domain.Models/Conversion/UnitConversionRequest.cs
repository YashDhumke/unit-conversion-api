namespace UnitConversion.Domain.Models.Conversion;

public class UnitConversionRequest
{
    public decimal Value { get; set; }
    public string FromUnit { get; set; } = string.Empty;
    public string ToUnit { get; set; } = string.Empty;
    public int? Precision { get; set; }
}
