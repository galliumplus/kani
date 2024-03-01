namespace Kanmi.Protocols;

/// <summary>
/// Une énumération riche qui liste les différents messages qui peuvent être reçus du lecteur. 
/// </summary>
public abstract record Message
{
    /// <summary>
    /// Le message n'a pas été compris.
    /// </summary>
    public record UnknownIntent() : Message;
    
    /// <summary>
    /// La connexion a été interrompue, ce message sera le dernier.
    /// </summary>
    public record ConnectionEnded() : Message;
    
    /// <summary>
    /// Un message indiquant qu'un échange avec une carte a commencé.
    /// Voir <see cref="ICardReaderListener.OnEngagedWithPicc"/> pour plus d'informations.
    /// </summary>
    public record EngagedWithPiccMessage(PiccUid Uid) : Message;
}