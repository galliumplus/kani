namespace Kanmi;

/// <summary>
/// Peut recevoir les notifications de lecteurs de cartes.
/// </summary>
public interface ICardReaderListener
{
    /// <summary>
    /// Appelé quand un nouveau lecteur est connecté.
    /// </summary>
    /// <param name="readerInfos">Une ligne contenant des informations sur le lecteur et le protocole utilisés.</param>
    void OnReaderConnected(string readerInfos);

    /// <summary>
    /// Appelé quand la connexion avec le lecteur actuel est interrompue.
    /// </summary>
    void OnReaderDisconnected();

    /// <summary>
    /// Appelé quand un échange commence entre le lecteur et une carte.
    /// </summary>
    /// <param name="uid">L'identifiant unique de la carte.</param>
    void OnEngagedWithPicc(PiccUid uid);

    /// <summary>
    /// Appelé quand une exception non gérée survient. Si la propriété <see cref="ErrorContext.WasHandled"/> du
    /// paramètre <paramref name="error"/> est toujours fausse une fois toutes les notifications envoyées, le service
    /// sera interrompu par l'exception. 
    /// </summary>
    /// <param name="error">Les informations sur l'erreur.</param>
    void OnUnexpectedError(ErrorContext error);
}