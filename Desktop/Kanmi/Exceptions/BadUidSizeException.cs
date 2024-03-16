namespace Kanmi.Exceptions;

/// <summary>
/// Levée quand la longueur d'un UID est invalide.
/// </summary>
public class BadUidSizeException : BadUidException
{
    /// <summary>
    /// Crée une exception qui indique que la longueur d'un UID est invalide.
    /// </summary>
    /// <param name="actualLength">La longueur invalide.</param>
    public BadUidSizeException(int actualLength)
        : base($"Un UID ne peut pas avoir une taille de {actualLength}")
    {
    }
}