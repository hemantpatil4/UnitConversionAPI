namespace UnitConversionApi.Exceptions;

/// <summary>
/// Raised when a conversion request is numerically, physically, or categorically invalid.
/// The message is safe to return to API clients.
/// </summary>
public sealed class UnitConversionException : Exception
{
    /// <summary>
    /// Creates an exception with a client-facing message.
    /// </summary>
    public UnitConversionException(string message)
        : base(message)
    {
    }
}
