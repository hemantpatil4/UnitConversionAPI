using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Exceptions;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

namespace UnitConversionApi.Controllers;

/// <summary>
/// Converts a numeric value from one unit to another.
/// </summary>
[ApiController]
[Route("api/v1/conversions")]
public sealed class ConversionsController : ControllerBase
{
    private readonly IUnitConversionService _conversionService;

    /// <summary>
    /// Creates the controller.
    /// </summary>
    public ConversionsController(IUnitConversionService conversionService)
    {
        _conversionService = conversionService;
    }

    /// <summary>
    /// Converts <c>value</c> from the <c>from</c> unit to the <c>to</c> unit.
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ConversionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public ActionResult<ConversionResponse> Get([FromQuery] ConversionRequest request)
    {
        if (request.Value is null)
        {
            return ApiErrorResults.BadRequest("Query parameter 'value' is required.");
        }

        if (string.IsNullOrWhiteSpace(request.From))
        {
            return ApiErrorResults.BadRequest("Query parameter 'from' is required.");
        }

        if (string.IsNullOrWhiteSpace(request.To))
        {
            return ApiErrorResults.BadRequest("Query parameter 'to' is required.");
        }

        try
        {
            var response = _conversionService.Convert(request.Value.Value, request.From, request.To);
            return Ok(response);
        }
        catch (UnitConversionException exception)
        {
            return ApiErrorResults.BadRequest(exception.Message);
        }
    }
}
