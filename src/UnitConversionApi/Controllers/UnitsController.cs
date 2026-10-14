using Microsoft.AspNetCore.Mvc;
using UnitConversionApi.Conversions;
using UnitConversionApi.Models;

namespace UnitConversionApi.Controllers;

/// <summary>
/// Lists the units accepted by the conversion endpoint.
/// </summary>
[ApiController]
[Route("api/v1/units")]
public sealed class UnitsController : ControllerBase
{
    private readonly UnitRegistry _registry;

    /// <summary>
    /// Creates the controller.
    /// </summary>
    public UnitsController(UnitRegistry registry)
    {
        _registry = registry;
    }

    /// <summary>
    /// Returns the registry: canonical name, category, and aliases for each unit.
    /// </summary>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(UnitCatalogResponse), StatusCodes.Status200OK)]
    public ActionResult<UnitCatalogResponse> Get()
    {
        var units = _registry.List()
            .Select(unit => new UnitCatalogItem
            {
                Name = unit.Definition.Name,
                Category = unit.Definition.Category.ToString(),
                Aliases = unit.Aliases
            })
            .ToArray();

        return Ok(new UnitCatalogResponse { Units = units });
    }
}
