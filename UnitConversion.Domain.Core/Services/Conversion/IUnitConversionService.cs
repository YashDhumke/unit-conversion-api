using UnitConversion.Domain.Models.Conversion;

namespace UnitConversion.Domain.Core.Services.Conversion;

public interface IUnitConversionService
{
    Task<UnitConversionResult> ConvertAsync(UnitConversionRequest request);
    Task<IReadOnlyCollection<string>> GetCategoriesAsync();
    Task<IReadOnlyCollection<MeasurementUnit>> GetUnitsAsync(string? category);
}
