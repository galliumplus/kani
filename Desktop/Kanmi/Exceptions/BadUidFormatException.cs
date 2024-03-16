namespace Kanmi.Exceptions;

/// <summary>
/// Indique une erreur avec le format de l'UID d'une carte.
/// </summary>
public class BadUidFormatException : BadUidException
{
    /// <summary>
    /// Crée une exception indiquant une erreur avec le format de l'UID d'une carte.
    /// </summary>
    /// <param name="uid">L'UID problématique.</param>
    public BadUidFormatException(string uid) : base($"« {uid} » n'est pas un UID valide.") { }
}