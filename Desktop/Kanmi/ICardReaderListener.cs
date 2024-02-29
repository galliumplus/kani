namespace Kanmi;

/// <summary>
/// Peut recevoir les notifications de lecteurs de cartes.
/// </summary>
public interface ICardReaderListener
{
    /// <summary>
    /// Appelé quand un nouveau lecteur est connecté.
    /// </summary>
    /// <param name="kanmiProtocolVersion">
    /// La version du protocole de communication utilisé par le lecteur. Actuellement, seule la version 2 (0x02) est
    /// supportée.
    /// </param>
    /// <param name="mfrc522FirmwareVersion">
    /// La version du micrologiciel du lecteur. Peut être :
    /// <ul>
    ///   <li>0x88 pour un clone de Fudan Semiconductor FM17522.</li>
    ///   <li>0x90 pour un MFRC522 v0.0</li>
    ///   <li>0x91 pour un MFRC522 v1.0</li>
    ///   <li>0x92 pour un MFRC522 v2.0</li>
    /// </ul>
    /// </param>
    void OnReaderConnected(byte kanmiProtocolVersion, byte mfrc522FirmwareVersion);

    /// <summary>
    /// Appelé quand la connexion avec le lecteur actuel est interrompue.
    /// </summary>
    void OnReaderDisconnected();

    /// <summary>
    /// Appelé quand un échange commence entre le lecteur et une carte.
    /// </summary>
    /// <param name="uid">L'identifiant unique de la carte.</param>
    void OnEngagedWithPicc(PiccUid uid);
}