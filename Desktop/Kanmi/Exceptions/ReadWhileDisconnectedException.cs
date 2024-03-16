namespace Kanmi.Exceptions;

/// <summary>
/// Cette exception est une erreur interne indiquant qu'une opération de lecture a été tentée sur un port fermé.
/// Ce n'est pas une <see cref="KanmiException"/>.
/// </summary>
public class ReadWhileDisconnectedException: InvalidOperationException
{
    /// <summary>
    /// Crée une instance de cette exception avec un message par défaut.
    /// </summary>
    public ReadWhileDisconnectedException() : base("Aucune connexion n'a été établie.")
    {
    }
}