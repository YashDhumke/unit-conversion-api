using UnitConversion.ApiModels.Conversion;
using UnitConversion.Domain.Models.Conversion;

namespace UnitConversion.Api.ApiServices.Conversion;

public interface IUnitConversionApiService : IApiService
{
    Task<UnitConversionResult> ConvertAsync(UnitConversionApiModel model);
    Task<IReadOnlyCollection<string>> GetCategoriesAsync();
    Task<IReadOnlyCollection<MeasurementUnit>> GetUnitsAsync(string? category);
}
