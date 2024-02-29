namespace Kanmi.Exceptions;

/// <summary>
/// Indique une erreur avec le format de l'UID d'une carte.
/// </summary>
public class BadUidFormatException : KanmiException
{
    /// <inheritdoc />
    public BadUidFormatException(string message) : base(message) { }
}