namespace UnitConversionApi.Conversions;

/// <summary>
/// In-memory catalog of supported units. Lookup is case-insensitive.
/// </summary>
public sealed class UnitRegistry
{
    private readonly Dictionary<string, UnitDefinition> _units;
    private readonly List<UnitDefinition> _definitions = new();
    private readonly Dictionary<string, List<string>> _aliases = new(StringComparer.Ordinal);

    /// <summary>
    /// Creates the built-in catalog.
    /// </summary>
    public UnitRegistry()
    {
        var definitions = new[]
        {
            new UnitDefinition("meter", UnitCategory.Length, 1d),
            new UnitDefinition("kilometer", UnitCategory.Length, 1000d),
            new UnitDefinition("centimeter", UnitCategory.Length, 0.01d),
            new UnitDefinition("millimeter", UnitCategory.Length, 0.001d),
            new UnitDefinition("mile", UnitCategory.Length, 1609.344d),
            new UnitDefinition("yard", UnitCategory.Length, 0.9144d),
            new UnitDefinition("foot", UnitCategory.Length, 0.3048d),
            new UnitDefinition("inch", UnitCategory.Length, 0.0254d),

            new UnitDefinition("kilogram", UnitCategory.Mass, 1d),
            new UnitDefinition("gram", UnitCategory.Mass, 0.001d),
            new UnitDefinition("milligram", UnitCategory.Mass, 0.000001d),
            new UnitDefinition("metric ton", UnitCategory.Mass, 1000d),
            new UnitDefinition("pound", UnitCategory.Mass, 0.45359237d),
            new UnitDefinition("ounce", UnitCategory.Mass, 0.028349523125d),

            new UnitDefinition("celsius", UnitCategory.Temperature, 0d),
            new UnitDefinition("fahrenheit", UnitCategory.Temperature, 0d),
            new UnitDefinition("kelvin", UnitCategory.Temperature, 0d)
        };

        _units = new Dictionary<string, UnitDefinition>(StringComparer.OrdinalIgnoreCase);
        foreach (var definition in definitions)
        {
            _definitions.Add(definition);
            _units.Add(definition.Name, definition);
            var compactName = definition.Name.Replace(" ", "", StringComparison.Ordinal);
            if (!compactName.Equals(definition.Name, StringComparison.Ordinal))
            {
                _units.Add(compactName, definition);
                if (!_aliases.TryGetValue(definition.Name, out var aliases))
                {
                    aliases = new List<string>();
                    _aliases.Add(definition.Name, aliases);
                }

                aliases.Add(compactName);
            }
        }
    }

    /// <summary>
    /// Returns each unit once, using its canonical name, in catalog order.
    /// </summary>
    public IReadOnlyList<RegisteredUnit> List()
    {
        return _definitions
            .Select(definition => new RegisteredUnit(
                definition,
                _aliases.TryGetValue(definition.Name, out var aliases)
                    ? aliases
                    : Array.Empty<string>()))
            .ToArray();
    }

    /// <summary>
    /// Resolves a unit name. Surrounding whitespace is ignored.
    /// </summary>
    public bool TryGet(string? name, out UnitDefinition? unit)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            unit = null;
            return false;
        }

        return _units.TryGetValue(name.Trim(), out unit);
    }
}
