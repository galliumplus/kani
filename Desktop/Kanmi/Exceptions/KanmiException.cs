namespace Kanmi.Exceptions;

/// <summary>
/// La classe de base pour toutes les exception propres à la bibliothèque.
/// </summary>
public abstract class KanmiException: Exception
{
    /// <summary>
    /// Crée une nouvelle instance de cette exception avec un message d'erreur donné.
    /// </summary>
    /// <param name="message">Un message décrivant le problème.</param>
    protected KanmiException(string message) : base(message) { }
}