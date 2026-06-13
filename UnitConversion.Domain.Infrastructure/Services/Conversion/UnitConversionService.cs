using UnitConversion.Domain.Core.Repositories.Conversion;
using UnitConversion.Domain.Core.Services.Conversion;
using UnitConversion.Domain.Models;
using UnitConversion.Domain.Models.Conversion;

namespace UnitConversion.Domain.Infrastructure.Services.Conversion;

public class UnitConversionService( IUnitCatalogRepository unitCatalogRepository) : IUnitConversionService
{
    public async Task<UnitConversionResult> ConvertAsync(UnitConversionRequest request)
    {
        var validationErrorList = ValidateRequest(request);

        if (validationErrorList.Any())
            throw new DomainValidationException(validationErrorList);

        var fromUnit = await unitCatalogRepository.GetByCodeAsync(request.FromUnit).ConfigureAwait(false);
        var toUnit = await unitCatalogRepository.GetByCodeAsync(request.ToUnit).ConfigureAwait(false);

        validationErrorList = ValidateUnits(request, fromUnit, toUnit);

        if (validationErrorList.Any())
            throw new DomainValidationException(validationErrorList);

        var baseValue = fromUnit!.ConvertToBase(request.Value);
        var convertedValue = toUnit!.ConvertFromBase(baseValue);

        return new UnitConversionResult
        {
            Category = fromUnit.Category,
            FromUnit = fromUnit,
            ToUnit = toUnit,
            InputValue = request.Value,
            ConvertedValue = decimal.Round(convertedValue, request.Precision ?? 6, MidpointRounding.AwayFromZero)
        };
    }

    public async Task<IReadOnlyCollection<string>> GetCategoriesAsync()
    {
        var units = await unitCatalogRepository.GetAllAsync().ConfigureAwait(false);
        return units.Select(x => x.Category).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
    }

    public async Task<IReadOnlyCollection<MeasurementUnit>> GetUnitsAsync(string? category)
    {
        var units = await unitCatalogRepository.GetAllAsync().ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(category))
            return units.OrderBy(x => x.Category).ThenBy(x => x.Name).ToList();

        return units.Where(x => x.Category.Equals(category.Trim(), StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Name).ToList();
    }

    private static List<DomainValidationResult> ValidateRequest(UnitConversionRequest? request)
    {
        var resultList = new List<DomainValidationResult>();

        if (request == null)
        {
            resultList.Add(DomainValidationResult.Failure("Request body is required.", nameof(UnitConversionRequest)));
            return resultList;
        }

        if (string.IsNullOrWhiteSpace(request.FromUnit))
            resultList.Add(DomainValidationResult.Failure("From unit is required.", nameof(request.FromUnit)));

        if (string.IsNullOrWhiteSpace(request.ToUnit))
            resultList.Add(DomainValidationResult.Failure("To unit is required.", nameof(request.ToUnit)));

        if (request.Precision is < 0 or > 12)
            resultList.Add(DomainValidationResult.Failure("Precision must be between 0 and 12.", nameof(request.Precision)));

        return resultList;
    }

    private static List<DomainValidationResult> ValidateUnits(UnitConversionRequest request, MeasurementUnit? fromUnit, MeasurementUnit toUnit)
    {
        var resultList = new List<DomainValidationResult>();

        if (fromUnit == null)
            resultList.Add(DomainValidationResult.Failure($"Unsupported from unit '{request.FromUnit}'.", nameof(request.FromUnit)));

        if (toUnit == null)
            resultList.Add(DomainValidationResult.Failure($"Unsupported to unit '{request.ToUnit}'.", nameof(request.ToUnit)));

        if (fromUnit != null && toUnit != null && !fromUnit.Category.Equals(toUnit.Category, StringComparison.OrdinalIgnoreCase))
        {
            resultList.Add(DomainValidationResult.Failure(
                $"Cannot convert from {fromUnit.Category} to {toUnit.Category}.",
                nameof(request.ToUnit)));
        }

        return resultList;
    }
}
