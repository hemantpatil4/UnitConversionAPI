namespace UnitConversionApi.Conversions;

/// <summary>
/// A catalog entry: one canonical unit and the extra names that resolve to it.
/// </summary>
public sealed class RegisteredUnit
{
    /// <summary>
    /// Creates a catalog entry.
    /// </summary>
    public RegisteredUnit(UnitDefinition definition, IReadOnlyList<string> aliases)
    {
        Definition = definition;
        Aliases = aliases;
    }

    /// <summary>Canonical unit.</summary>
    public UnitDefinition Definition { get; }

    /// <summary>Accepted spellings other than the canonical name.</summary>
    public IReadOnlyList<string> Aliases { get; }
}
