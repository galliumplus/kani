namespace Kanmi;

/// <summary>
/// Envoie des notifications quand des informations sont reçues depuis un lecteur. 
/// </summary>
public interface ICardReaderService
{
    /// <summary>
    /// Indique si ce service est en cours d'exécution et peut envoyer des notifications.
    /// </summary>
    bool IsActive { get; }
    
    /// <summary>
    /// Abonne <paramref name="listener"/> aux notifications de ce service.
    /// </summary>
    /// <param name="listener">L'objet à qui envoyer les notifications.</param>
    void Subscribe(ICardReaderListener listener);
    
    /// <summary>
    /// Désabonne <paramref name="listener"/> des notifications de ce service.
    /// </summary>
    /// <param name="listener">L'objet à qui arrêter d'envoyer les notifications.</param>
    void Unsubscribe(ICardReaderListener listener);

    /// <summary>
    /// (Re)Démarre le service s'il est arrêté.
    /// </summary>
    void Start();
    
    /// <summary>
    /// Mets le service en pause s'il ne l'est pas déjà.
    /// </summary>
    void Stop();
}