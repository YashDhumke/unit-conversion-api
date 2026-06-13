using UnitConversion.Domain.Infrastructure.Repositories.Conversion;
using UnitConversion.Domain.Models.Conversion;

namespace UnitConversion.Tests.Conversion;

public class InMemoryUnitCatalogRepositoryTests
{
    private readonly InMemoryUnitCatalogRepository _repository = new();

    [Fact]
    public async Task GetAllAsync_ReturnsAllUnits()
    {
        var units = await _repository.GetAllAsync();
        Assert.NotEmpty(units);
    }

    [Theory]
    [InlineData("m")]
    [InlineData("km")]
    [InlineData("ft")]
    [InlineData("kg")]
    [InlineData("lb")]
    [InlineData("c")]
    [InlineData("f")]
    [InlineData("k")]
    [InlineData("l")]
    [InlineData("s")]
    public async Task GetByCodeAsync_KnownCode_ReturnsUnit(string code)
    {
        var unit = await _repository.GetByCodeAsync(code);
        Assert.NotNull(unit);
        Assert.Equal(code, unit.Code, ignoreCase: true);
    }

    [Fact]
    public async Task GetByCodeAsync_UnknownCode_ReturnsNull()
    {
        var unit = await _repository.GetByCodeAsync("unknown");
        Assert.Null(unit);
    }

    [Fact]
    public async Task GetByCodeAsync_IsCaseInsensitive()
    {
        var lower = await _repository.GetByCodeAsync("km");
        var upper = await _repository.GetByCodeAsync("KM");
        Assert.NotNull(lower);
        Assert.Equal(lower!.Code, upper!.Code);
    }

    [Theory]
    [InlineData(ConversionCategory.Length)]
    [InlineData(ConversionCategory.Weight)]
    [InlineData(ConversionCategory.Temperature)]
    [InlineData(ConversionCategory.Volume)]
    [InlineData(ConversionCategory.Time)]
    public async Task GetAllAsync_ContainsExpectedCategories(string category)
    {
        var units = await _repository.GetAllAsync();
        Assert.Contains(units, u => u.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetAllAsync_HasAtLeastFiveCategories()
    {
        var units = await _repository.GetAllAsync();
        var categories = units.Select(u => u.Category).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        Assert.True(categories.Count >= 5);
    }
}
