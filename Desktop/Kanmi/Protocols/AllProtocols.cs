using Kanmi.Protocols.Implementations;

namespace Kanmi.Protocols;

/// <summary>
/// Une classe regroupant tous les protocoles disponibles.
/// </summary>
public static class AllProtocols
{
    private static readonly IProtocol[] protocolsList = { new V2Protocol() };

    /// <summary>
    /// Renvoie une liste des protocoles disponibles.
    /// </summary>
    public static IEnumerable<IProtocol> ProtocolsList => protocolsList;
}