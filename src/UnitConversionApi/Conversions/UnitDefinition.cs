namespace UnitConversionApi.Conversions;

/// <summary>
/// One supported unit and the metadata required to convert it.
/// </summary>
public sealed class UnitDefinition
{
    /// <summary>
    /// Creates a unit definition.
    /// </summary>
    /// <param name="name">Canonical name returned by the API.</param>
    /// <param name="category">Category the unit belongs to.</param>
    /// <param name="factorToBase">
    /// Multiplier that converts a quantity in this unit into the category base unit.
    /// Unused for temperature, which is converted with dedicated formulas.
    /// </param>
    public UnitDefinition(string name, UnitCategory category, double factorToBase)
    {
        Name = name;
        Category = category;
        FactorToBase = factorToBase;
    }

    /// <summary>Canonical unit name.</summary>
    public string Name { get; }

    /// <summary>Category this unit belongs to.</summary>
    public UnitCategory Category { get; }

    /// <summary>
    /// Quantity of the base unit represented by 1 of this unit.
    /// Meter is the length base and kilogram is the mass base.
    /// </summary>
    public double FactorToBase { get; }
}
