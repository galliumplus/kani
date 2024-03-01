using System.Diagnostics.CodeAnalysis;
using Kanmi.Serial;
using Kanmi.Serial.NetCore;

namespace Kanmi.Protocols;

/// <summary>
/// Représente un protocole utilisé pour communiquer avec un lecteur de carte.
/// </summary>
public interface IProtocol
{
    /// <summary>
    /// Un nom unique désignant le protocole.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Configure un port pour être utilisé avec ce protocole.
    /// </summary>
    /// <param name="port">Le port à configurer.</param>
    void ConfigurePort(IConfigurableSerialPort port);

    /// <summary>
    /// Vérifie si ce protocole est celui utilisé par un lecteur de carte. 
    /// </summary>
    /// <param name="firstMessage">Le premier message envoyé par le lecteur.</param>
    /// <returns><see langword="true"/> si ce protocole peut être utilisé.</returns>
    bool CanHandle(string firstMessage);

    /// <summary>
    /// Extrait et mets en forme les informations sur le matériel et ce protocole.
    /// </summary>
    /// <param name="firstMessage">Le premier message envoyé par le lecteur.</param>
    /// <returns>Une ligne de texte.</returns>
    string ExtractReaderInfos(string firstMessage);

    /// <summary>
    /// Analyse 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Message ParseMessage(string message);
}