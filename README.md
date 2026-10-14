# Unit Conversion API

ASP.NET Core Web API that converts a numeric value between units of length, mass, and temperature. Unit definitions live in code. The API does not use a database.

The installed SDK is .NET 8.0.411, so the projects target `net8.0`.

## Features

`GET /api/v1/units` lists the registry: canonical name, category, and aliases.

`GET /api/v1/conversions?value={value}&from={sourceUnit}&to={targetUnit}`

- Case-insensitive unit names.
- Decimal and negative inputs where the conversion is mathematically valid.
- A unit can be converted to itself.
- Length and mass scale through a base unit.
- Temperature uses affine formulas and rejects values below absolute zero.
- HTTP 200 for success and HTTP 400 with a consistent JSON error body for invalid input.
- Swagger UI in the Development environment.

### Length

Base unit: meter.

| Unit | Canonical name | Factor to meters |
| --- | --- | --- |
| Meter | `meter` | 1 |
| Kilometer | `kilometer` | 1000 |
| Centimeter | `centimeter` | 0.01 |
| Millimeter | `millimeter` | 0.001 |
| Mile | `mile` | 1609.344 (international mile) |
| Yard | `yard` | 0.9144 |
| Foot | `foot` | 0.3048 |
| Inch | `inch` | 0.0254 |

### Mass

Base unit: kilogram.

| Unit | Canonical name | Factor to kilograms |
| --- | --- | --- |
| Kilogram | `kilogram` | 1 |
| Gram | `gram` | 0.001 |
| Milligram | `milligram` | 0.000001 |
| Metric ton | `metric ton` | 1000 |
| Pound | `pound` | 0.45359237 (avoirdupois) |
| Ounce | `ounce` | 0.028349523125 |

`metricton` is accepted as an alias of `metric ton`. Responses always use the canonical name.

### Temperature

Converted through Celsius. These are offsets, not scale factors.

- Fahrenheit = Celsius * 9 / 5 + 32
- Celsius = (Fahrenheit - 32) * 5 / 9
- Kelvin = Celsius + 273.15
- Celsius = Kelvin - 273.15

Absolute zero is accepted. Colder values are rejected:

- Celsius below -273.15
- Fahrenheit below -459.67
- Kelvin below 0

## Architecture

```
UnitConversionApi.sln
src/UnitConversionApi/          ASP.NET Core Web API
tests/UnitConversionApi.Tests/  xUnit service and HTTP tests
```

`ConversionsController` checks that `value`, `from`, and `to` are present, then calls `IUnitConversionService`. The service looks up both units in `UnitRegistry`, rejects unknown units and mixed categories, and delegates to `LengthConverter`, `MassConverter`, or `TemperatureConverter`.

Length and mass share one scale formula: `value * fromFactor / toFactor`. Temperature stays in its own converter so a new linear unit does not require a new pair-wise branch.

Invalid model binding and `UnitConversionException` both return `ApiErrorResponse` (`type`, `title`, `status`, `detail`). Stack traces are not included.

`double` (IEEE-754 binary64) is used for every calculation. Intermediate results are not rounded. JSON serialization writes the computed value.

## Prerequisites

- .NET SDK 8.0.411 or a later 8.0 feature band (`global.json` allows `latestFeature` roll-forward)
- No database

## Commands

From the repository root:

```bash
dotnet restore UnitConversionApi.sln
dotnet build UnitConversionApi.sln
dotnet test UnitConversionApi.sln
dotnet run --project src/UnitConversionApi/UnitConversionApi.csproj --launch-profile http
```

The HTTP profile listens on `http://localhost:5109`. Swagger UI is at `http://localhost:5109/swagger`.

## Example requests

List canonical names:

```bash
curl "http://localhost:5109/api/v1/units"
```

```json
{
  "units": [
    { "name": "meter", "category": "Length", "aliases": [] },
    { "name": "metric ton", "category": "Mass", "aliases": ["metricton"] },
    { "name": "celsius", "category": "Temperature", "aliases": [] }
  ]
}
```

The live response includes every registered unit. `name` is the canonical spelling used in conversion responses.

Convert 100 meters to feet:

```bash
curl "http://localhost:5109/api/v1/conversions?value=100&from=meter&to=foot"
```

```json
{
  "value": 100,
  "fromUnit": "meter",
  "toUnit": "foot",
  "result": 328.0839895013123,
  "category": "Length"
}
```

`result` is `100 / 0.3048` with no manual rounding. The exact JSON digits follow `double` serialization.

## Example errors

Unknown unit:

```bash
curl "http://localhost:5109/api/v1/conversions?value=1&from=furlong&to=meter"
```

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Invalid request.",
  "status": 400,
  "detail": "Unknown source unit 'furlong'."
}
```

Different categories:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Invalid request.",
  "status": 400,
  "detail": "Cannot convert 'meter' (Length) to 'kilogram' (Mass) because the units belong to different categories."
}
```

Malformed number, missing query parameters, non-finite values (`NaN`, `Infinity`), and temperatures below absolute zero also return HTTP 400 with the same shape.

## Floating-point precision

Factors such as `0.3048` and `0.45359237` are not all exact in binary. Results are deterministic for a given input, but they are binary approximations of the decimal definitions. Tests compare with 9 decimal places, or with a small range at absolute zero, where the Fahrenheit literal `-459.67` is not exact in `double`. A tolerance of `1e-6` kelvin is used only to accept that boundary, not to round the returned result.

## Adding a unit

Length or mass:

1. Add one `UnitDefinition` in `UnitRegistry` with a factor relative to meters or kilograms.
2. Add a service test for a known value.

No controller or pair-wise switch changes are required. The canonical name is the registry key. A name that contains a space should also be registered without the space if you want a single-token query alias.

Temperature needs a formula branch in `TemperatureConverter`, because it is not a scale factor. Also extend the absolute-zero check if the new unit has its own zero point.

## Limitations

- Units are a fixed in-memory catalog.
- Only length, mass, and temperature are supported.
- The mile is the international mile, not the US survey mile.
- The pound and ounce are avoirdupois.
- Negative length and mass are accepted because the scale conversion is linear. The API does not decide whether a negative mass is meaningful in a particular domain.
- There is no prefix parser (`km`, `m`, `kg`) and no compound units.
- Swagger is enabled only when `ASPNETCORE_ENVIRONMENT` is `Development`, which `dotnet run` uses by default.
