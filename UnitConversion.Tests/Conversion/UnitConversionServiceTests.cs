using NSubstitute;
using UnitConversion.Domain.Core.Repositories.Conversion;
using UnitConversion.Domain.Infrastructure.Services.Conversion;
using UnitConversion.Domain.Models;
using UnitConversion.Domain.Models.Conversion;

namespace UnitConversion.Tests.Conversion;

public class UnitConversionServiceTests
{
    private readonly IUnitCatalogRepository _repository = Substitute.For<IUnitCatalogRepository>();
    private readonly UnitConversionService _service;

    private static readonly MeasurementUnit Meter = new("m", "Meter", "m", "length", 1m);
    private static readonly MeasurementUnit Foot = new("ft", "Foot", "ft", "length", 0.3048m);
    private static readonly MeasurementUnit Kilogram = new("kg", "Kilogram", "kg", "weight", 1m);
    private static readonly MeasurementUnit Celsius = new("c", "Celsius", "degC", "temperature", 1m);
    private static readonly MeasurementUnit Fahrenheit = new("f", "Fahrenheit", "degF", "temperature", 5m / 9m, -32m);
    private static readonly MeasurementUnit Kelvin = new("k", "Kelvin", "K", "temperature", 1m, -273.15m);

    public UnitConversionServiceTests()
    {
        _service = new UnitConversionService(_repository);
    }

    // --- ConvertAsync: success cases ---

    [Fact]
    public async Task ConvertAsync_SameUnit_ReturnsSameValue()
    {
        _repository.GetByCodeAsync("m").Returns(Meter);

        var result = await _service.ConvertAsync(new UnitConversionRequest
        {
            Value = 5m,
            FromUnit = "m",
            ToUnit = "m"
        });

        Assert.Equal(5m, result.ConvertedValue);
    }

    [Fact]
    public async Task ConvertAsync_MetersToFeet_ReturnsCorrectValue()
    {
        _repository.GetByCodeAsync("m").Returns(Meter);
        _repository.GetByCodeAsync("ft").Returns(Foot);

        var result = await _service.ConvertAsync(new UnitConversionRequest
        {
            Value = 1m,
            FromUnit = "m",
            ToUnit = "ft",
            Precision = 4
        });

        Assert.Equal(3.2808m, result.ConvertedValue);
    }

    [Fact]
    public async Task ConvertAsync_CelsiusToFahrenheit_ReturnsCorrectValue()
    {
        _repository.GetByCodeAsync("c").Returns(Celsius);
        _repository.GetByCodeAsync("f").Returns(Fahrenheit);

        var result = await _service.ConvertAsync(new UnitConversionRequest
        {
            Value = 0m,
            FromUnit = "c",
            ToUnit = "f",
            Precision = 2
        });

        Assert.Equal(32m, result.ConvertedValue);
    }

    [Fact]
    public async Task ConvertAsync_FahrenheitToCelsius_ReturnsCorrectValue()
    {
        _repository.GetByCodeAsync("f").Returns(Fahrenheit);
        _repository.GetByCodeAsync("c").Returns(Celsius);

        var result = await _service.ConvertAsync(new UnitConversionRequest
        {
            Value = 212m,
            FromUnit = "f",
            ToUnit = "c",
            Precision = 2
        });

        Assert.Equal(100m, result.ConvertedValue);
    }

    [Fact]
    public async Task ConvertAsync_CelsiusToKelvin_ReturnsCorrectValue()
    {
        _repository.GetByCodeAsync("c").Returns(Celsius);
        _repository.GetByCodeAsync("k").Returns(Kelvin);

        var result = await _service.ConvertAsync(new UnitConversionRequest
        {
            Value = 0m,
            FromUnit = "c",
            ToUnit = "k",
            Precision = 2
        });

        Assert.Equal(273.15m, result.ConvertedValue);
    }

    [Fact]
    public async Task ConvertAsync_PrecisionIsRespected()
    {
        _repository.GetByCodeAsync("m").Returns(Meter);
        _repository.GetByCodeAsync("ft").Returns(Foot);

        var result = await _service.ConvertAsync(new UnitConversionRequest
        {
            Value = 1m,
            FromUnit = "m",
            ToUnit = "ft",
            Precision = 2
        });

        Assert.Equal(2, GetDecimalPlaces(result.ConvertedValue));
    }

    [Fact]
    public async Task ConvertAsync_DefaultPrecisionIsSix()
    {
        _repository.GetByCodeAsync("m").Returns(Meter);
        _repository.GetByCodeAsync("ft").Returns(Foot);

        var result = await _service.ConvertAsync(new UnitConversionRequest
        {
            Value = 1m,
            FromUnit = "m",
            ToUnit = "ft"
        });

        Assert.True(GetDecimalPlaces(result.ConvertedValue) <= 6);
    }

    [Fact]
    public async Task ConvertAsync_ResultContainsCorrectMetadata()
    {
        _repository.GetByCodeAsync("m").Returns(Meter);
        _repository.GetByCodeAsync("ft").Returns(Foot);

        var result = await _service.ConvertAsync(new UnitConversionRequest
        {
            Value = 1m,
            FromUnit = "m",
            ToUnit = "ft"
        });

        Assert.Equal("length", result.Category);
        Assert.Equal(Meter, result.FromUnit);
        Assert.Equal(Foot, result.ToUnit);
        Assert.Equal(1m, result.InputValue);
    }

    // --- ConvertAsync: validation failures ---

    [Fact]
    public async Task ConvertAsync_NullRequest_ThrowsDomainValidationException()
    {
        await Assert.ThrowsAsync<DomainValidationException>(() =>
            _service.ConvertAsync(null!));
    }

    [Fact]
    public async Task ConvertAsync_EmptyFromUnit_ThrowsDomainValidationException()
    {
        var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
            _service.ConvertAsync(new UnitConversionRequest { Value = 1m, FromUnit = "", ToUnit = "ft" }));

        Assert.Contains(ex.ValidationResults, r => r.Source == nameof(UnitConversionRequest.FromUnit));
    }

    [Fact]
    public async Task ConvertAsync_EmptyToUnit_ThrowsDomainValidationException()
    {
        var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
            _service.ConvertAsync(new UnitConversionRequest { Value = 1m, FromUnit = "m", ToUnit = "" }));

        Assert.Contains(ex.ValidationResults, r => r.Source == nameof(UnitConversionRequest.ToUnit));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(13)]
    public async Task ConvertAsync_InvalidPrecision_ThrowsDomainValidationException(int precision)
    {
        var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
            _service.ConvertAsync(new UnitConversionRequest
            {
                Value = 1m,
                FromUnit = "m",
                ToUnit = "ft",
                Precision = precision
            }));

        Assert.Contains(ex.ValidationResults, r => r.Source == nameof(UnitConversionRequest.Precision));
    }

    [Fact]
    public async Task ConvertAsync_UnknownFromUnit_ThrowsDomainValidationException()
    {
        _repository.GetByCodeAsync("xyz").Returns((MeasurementUnit?)null);
        _repository.GetByCodeAsync("ft").Returns(Foot);

        var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
            _service.ConvertAsync(new UnitConversionRequest { Value = 1m, FromUnit = "xyz", ToUnit = "ft" }));

        Assert.Contains(ex.ValidationResults, r => r.Source == nameof(UnitConversionRequest.FromUnit));
    }

    [Fact]
    public async Task ConvertAsync_UnknownToUnit_ThrowsDomainValidationException()
    {
        _repository.GetByCodeAsync("m").Returns(Meter);
        _repository.GetByCodeAsync("xyz").Returns((MeasurementUnit?)null);

        var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
            _service.ConvertAsync(new UnitConversionRequest { Value = 1m, FromUnit = "m", ToUnit = "xyz" }));

        Assert.Contains(ex.ValidationResults, r => r.Source == nameof(UnitConversionRequest.ToUnit));
    }

    [Fact]
    public async Task ConvertAsync_CrossCategoryConversion_ThrowsDomainValidationException()
    {
        _repository.GetByCodeAsync("m").Returns(Meter);
        _repository.GetByCodeAsync("kg").Returns(Kilogram);

        var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
            _service.ConvertAsync(new UnitConversionRequest { Value = 1m, FromUnit = "m", ToUnit = "kg" }));

        Assert.Single(ex.ValidationResults);
    }

    // --- GetCategoriesAsync ---

    [Fact]
    public async Task GetCategoriesAsync_ReturnsDistinctSortedCategories()
    {
        _repository.GetAllAsync().Returns(new List<MeasurementUnit>
        {
            Meter, Foot, Kilogram, Celsius
        });

        var categories = await _service.GetCategoriesAsync();

        Assert.Equal(["length", "temperature", "weight"], categories.Order().ToList());
    }

    // --- GetUnitsAsync ---

    [Fact]
    public async Task GetUnitsAsync_NoFilter_ReturnsAllUnits()
    {
        _repository.GetAllAsync().Returns(new List<MeasurementUnit> { Meter, Foot, Kilogram });

        var units = await _service.GetUnitsAsync(null);

        Assert.Equal(3, units.Count);
    }

    [Fact]
    public async Task GetUnitsAsync_WithCategory_ReturnsFilteredUnits()
    {
        _repository.GetAllAsync().Returns(new List<MeasurementUnit> { Meter, Foot, Kilogram });

        var units = await _service.GetUnitsAsync("length");

        Assert.Equal(2, units.Count);
        Assert.All(units, u => Assert.Equal("length", u.Category));
    }

    [Fact]
    public async Task GetUnitsAsync_CategoryIsCaseInsensitive()
    {
        _repository.GetAllAsync().Returns(new List<MeasurementUnit> { Meter, Foot, Kilogram });

        var units = await _service.GetUnitsAsync("LENGTH");

        Assert.Equal(2, units.Count);
    }

    [Fact]
    public async Task GetUnitsAsync_UnknownCategory_ReturnsEmptyList()
    {
        _repository.GetAllAsync().Returns(new List<MeasurementUnit> { Meter, Foot });

        var units = await _service.GetUnitsAsync("pressure");

        Assert.Empty(units);
    }

    private static int GetDecimalPlaces(decimal value)
    {
        return BitConverter.GetBytes(decimal.GetBits(value)[3])[2];
    }
}
