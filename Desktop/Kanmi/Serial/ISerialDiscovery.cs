namespace Kanmi.Serial;

/// <summary>
/// Permets la découverte des ports série.
/// </summary>
public interface ISerialDiscovery
{
    /// <summary>
    /// Renvoie la liste des nouveaux ports disponibles depuis le dernier appel.
    /// </summary>
    /// <returns>Un tableau contenant les nouveaux ports.</returns>
    ISerialPort[] ListNewlyAvailablePorts();
}