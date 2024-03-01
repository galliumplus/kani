using System.Diagnostics.CodeAnalysis;
using Kanmi.Protocols;

namespace Kanmi.Serial;

/// <summary>
/// Représente un port série et la communication à travers celui-ci si il est ouvert.
/// </summary>
public interface ISerialPort
{
    /// <summary>
    /// Indique si il est possible qu'un lecteur de carte soit connecté à ce port. 
    /// </summary>
    bool MayBeACardReader { get; }
    
    /// <summary>
    /// Indique si ce port existe toujours. 
    /// </summary>
    bool StillAvailable { get; }

    /// <summary>
    /// Donne des informations sur le lecteur connecté, s'il y en a un.
    /// </summary>
    string? ReaderInfos { get; }

    /// <summary>
    /// Tente d'établir une connexion avec un éventuel lecteur de carte connecté à ce port. Si cette méthode renvoie
    /// <see langword="true"/>, le port sera ouvert à la fin de l'appel.
    /// </summary>
    /// <returns><see langword="true"/> si la connexion a bien été établie.</returns>
    bool TryToConnect();

    /// <summary>
    /// Ferme le port s'il ne l'est pas déjà.
    /// </summary>
    void EnsureIsClosed();

    /// <summary>
    /// Attends jusqu'à recevoir un message du lecteur.
    /// </summary>
    /// <returns>Le message reçu.</returns>
    /// <exception cref="TimeoutException">
    /// Quand le délai d'attente est dépassé et qu'aucun message n'a pu être lu.
    /// </exception>
    Message ReadNextMessage();
}