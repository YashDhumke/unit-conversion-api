using UnitConversion.ApiModels.Conversion;
using UnitConversion.Domain.Core.Services.Conversion;
using UnitConversion.Domain.Models.Conversion;

namespace UnitConversion.Api.ApiServices.Conversion;

public class UnitConversionApiService( IUnitConversionService unitConversionService, ILogger<UnitConversionApiService> logger) :IUnitConversionApiService
{
    public async Task<UnitConversionResult> ConvertAsync(UnitConversionApiModel model)
    {
        logger.LogTrace("Conversion request: {@ConversionRequest}", model);

        var result = await unitConversionService.ConvertAsync(model).ConfigureAwait(false);
        return result;
    }

    public Task<IReadOnlyCollection<string>> GetCategoriesAsync()
    {
        return unitConversionService.GetCategoriesAsync();
    }

    public Task<IReadOnlyCollection<MeasurementUnit>> GetUnitsAsync(string? category)
    {
        return unitConversionService.GetUnitsAsync(category);
    }
}
