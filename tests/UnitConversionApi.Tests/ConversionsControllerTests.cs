using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using UnitConversionApi.Models;

namespace UnitConversionApi.Tests;

public class ConversionsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _client;

    public ConversionsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUnits_ReturnsCanonicalNamesAndAliases()
    {
        var response = await _client.GetAsync("/api/v1/units");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<UnitCatalogResponse>(JsonOptions);
        Assert.NotNull(body);

        var meter = Assert.Single(body.Units, unit => unit.Name == "meter");
        Assert.Equal("Length", meter.Category);
        Assert.Empty(meter.Aliases);

        var metricTon = Assert.Single(body.Units, unit => unit.Name == "metric ton");
        Assert.Equal("Mass", metricTon.Category);
        Assert.Equal("metricton", Assert.Single(metricTon.Aliases));

        Assert.Contains(body.Units, unit => unit.Name == "celsius" && unit.Category == "Temperature");
        Assert.Equal(17, body.Units.Count);
        Assert.Equal(body.Units.Count, body.Units.Select(unit => unit.Name).Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public async Task Get_ValidConversion_Returns200AndResult()
    {
        var response = await _client.GetAsync("/api/v1/conversions?value=100&from=meter&to=foot");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ConversionResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(100d, body.Value);
        Assert.Equal("meter", body.FromUnit);
        Assert.Equal("foot", body.ToUnit);
        Assert.Equal("Length", body.Category);
        Assert.Equal(100d / 0.3048d, body.Result, precision: 9);
    }

    [Fact]
    public async Task Get_UnknownUnit_Returns400()
    {
        var response = await _client.GetAsync("/api/v1/conversions?value=1&from=furlong&to=meter");

        var error = await ReadError(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(400, error.Status);
        Assert.Contains("Unknown source unit", error.Detail, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Get_IncompatibleCategories_Returns400()
    {
        var response = await _client.GetAsync("/api/v1/conversions?value=1&from=meter&to=kilogram");

        var error = await ReadError(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("different categories", error.Detail, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Get_MalformedValue_Returns400()
    {
        var response = await _client.GetAsync("/api/v1/conversions?value=abc&from=meter&to=foot");

        var error = await ReadError(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("not valid", error.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Get_MissingValue_Returns400()
    {
        var response = await _client.GetAsync("/api/v1/conversions?from=meter&to=foot");

        var error = await ReadError(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("value", error.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Get_TemperatureBelowAbsoluteZero_Returns400()
    {
        var response = await _client.GetAsync("/api/v1/conversions?value=-1&from=kelvin&to=celsius");

        var error = await ReadError(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("absolute zero", error.Detail, StringComparison.Ordinal);
        Assert.DoesNotContain("at ", error.Detail, StringComparison.Ordinal);
    }

    private static async Task<ApiErrorResponse> ReadError(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        var error = JsonSerializer.Deserialize<ApiErrorResponse>(body, JsonOptions);
        Assert.NotNull(error);
        return error;
    }
}
