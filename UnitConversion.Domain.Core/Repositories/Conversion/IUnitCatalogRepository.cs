using UnitConversion.Domain.Models.Conversion;

namespace UnitConversion.Domain.Core.Repositories.Conversion;

public interface IUnitCatalogRepository
{
    Task<IReadOnlyCollection<MeasurementUnit>> GetAllAsync();
    Task<MeasurementUnit?> GetByCodeAsync(string code);
}
