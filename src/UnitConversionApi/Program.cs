using UnitConversionApi.Conversions;
using UnitConversionApi.Models;
using UnitConversionApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<UnitRegistry>();
builder.Services.AddSingleton<LengthConverter>();
builder.Services.AddSingleton<MassConverter>();
builder.Services.AddSingleton<TemperatureConverter>();
builder.Services.AddSingleton<IUnitConversionService, UnitConversionService>();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var messages = context.ModelState
                .Where(entry => entry.Value is { Errors.Count: > 0 })
                .SelectMany(entry => entry.Value!.Errors.Select(error =>
                    string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? $"The value provided for '{entry.Key}' is invalid."
                        : error.ErrorMessage))
                .ToArray();

            var detail = messages.Length == 0
                ? "The request is invalid."
                : string.Join(" ", messages);

            return ApiErrorResults.BadRequest(detail);
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Unit Conversion API",
        Version = "v1",
        Description = "Converts numerical values between units of length, mass, and temperature."
    });
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new ApiErrorResponse
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            Title = "An unexpected error occurred.",
            Status = StatusCodes.Status500InternalServerError,
            Detail = "The request could not be completed."
        });
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

/// <summary>
/// Exposes the entry point to the integration test host.
/// </summary>
public partial class Program;
