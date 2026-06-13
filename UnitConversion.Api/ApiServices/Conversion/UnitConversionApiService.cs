using UnitConversion.Domain.Core.Services.Conversion;
using UnitConversion.Domain.Models.Conversion;

namespace UnitConversion.Api.ApiServices.Conversion;

public class UnitConversionApiService(IUnitConversionService unitConversionService, ILogger<UnitConversionApiService> logger) : IUnitConversionApiService
{
    public async Task<UnitConversionResult> ConvertAsync(UnitConversionRequest request)
    {
        logger.LogTrace("Conversion request: {@ConversionRequest}", request);

        var result = await unitConversionService.ConvertAsync(request).ConfigureAwait(false);
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
