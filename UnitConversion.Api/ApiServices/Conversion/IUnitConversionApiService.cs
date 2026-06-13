using UnitConversion.Domain.Models.Conversion;

namespace UnitConversion.Api.ApiServices.Conversion;

public interface IUnitConversionApiService
{
    Task<UnitConversionResult> ConvertAsync(UnitConversionRequest request);
    Task<IReadOnlyCollection<string>> GetCategoriesAsync();
    Task<IReadOnlyCollection<MeasurementUnit>> GetUnitsAsync(string? category);
}
