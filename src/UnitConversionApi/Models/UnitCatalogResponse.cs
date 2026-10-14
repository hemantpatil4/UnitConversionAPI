namespace UnitConversionApi.Models;

/// <summary>
/// Catalog of units the conversion endpoint accepts.
/// </summary>
public sealed class UnitCatalogResponse
{
    /// <summary>
    /// Registered units in catalog order. Each entry uses the canonical name.
    /// </summary>
    public IReadOnlyList<UnitCatalogItem> Units { get; init; } = Array.Empty<UnitCatalogItem>();
}

/// <summary>
/// One registered unit.
/// </summary>
public sealed class UnitCatalogItem
{
    /// <summary>
    /// Name returned by conversion responses and accepted in <c>from</c> and <c>to</c>.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Category this unit converts within.
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Extra accepted spellings. Conversion responses still use <see cref="Name"/>.
    /// </summary>
    public IReadOnlyList<string> Aliases { get; init; } = Array.Empty<string>();
}
