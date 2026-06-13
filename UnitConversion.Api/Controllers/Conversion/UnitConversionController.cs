using UnitConversion.Api.ApiServices.Conversion;
using UnitConversion.Domain.Models;
using UnitConversion.Domain.Models.Conversion;
using Microsoft.AspNetCore.Mvc;

namespace UnitConversion.Api.Controllers.Conversion;

[Route("api/v1/[controller]")]
[ApiController]
public class UnitConversionController : ControllerBase
{
    private readonly IUnitConversionApiService unitConversionApiService;

    public UnitConversionController(IUnitConversionApiService unitConversionApiService)
    {
        this.unitConversionApiService = unitConversionApiService;
    }

    [HttpPost("convert", Name = "ConvertUnit")]
    public async Task<ActionResult> Convert([FromBody] UnitConversionRequest request)
    {
        if (request == null)
            return BadRequest("Conversion request is required.");

        try
        {
            var result = await unitConversionApiService.ConvertAsync(request).ConfigureAwait(false);
            return Ok(result);
        }
        catch (DomainValidationException ex)
        {
            return BadRequest(new { message = ex.Message, errors = ex.ValidationResults });
        }
    }

    [HttpGet("categories", Name = "GetConversionCategories")]
    public async Task<ActionResult> GetCategories()
    {
        var result = await unitConversionApiService.GetCategoriesAsync().ConfigureAwait(false);
        return Ok(result);
    }

    [HttpGet("units", Name = "GetConversionUnits")]
    public async Task<ActionResult> GetUnits([FromQuery] string? category)
    {
        var result = await unitConversionApiService.GetUnitsAsync(category).ConfigureAwait(false);
        return Ok(result);
    }
}
