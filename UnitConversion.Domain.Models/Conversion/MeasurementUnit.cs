namespace UnitConversion.Domain.Models.Conversion;

public class MeasurementUnit
{
    public MeasurementUnit(
        string code,
        string name,
        string symbol,
        string category,
        decimal factorToBase,
        decimal offsetToBase = 0m)
    {
        Code = code;
        Name = name;
        Symbol = symbol;
        Category = category;
        FactorToBase = factorToBase;
        OffsetToBase = offsetToBase;
    }

    public string Code { get; }
    public string Name { get; }
    public string Symbol { get; }
    public string Category { get; }
    public decimal FactorToBase { get; }
    public decimal OffsetToBase { get; }

    public decimal ConvertToBase(decimal value)
    {
        return (value + OffsetToBase) * FactorToBase;
    }

    public decimal ConvertFromBase(decimal baseValue)
    {
        return (baseValue / FactorToBase) - OffsetToBase;
    }
}
