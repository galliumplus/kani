using System.Text;

namespace Kanmi.Serial;

/// <summary>
/// Un port série qui peut être configuré.
/// </summary>
public interface IConfigurableSerialPort
{
    /// <summary>
    /// Fixe la vitesse de transmission.
    /// </summary>
    /// <param name="rate">La vitesse souhaitée en baud.</param>
    void SetBaudRate(int rate);

    /// <summary>
    /// Choisis l'encodage à utiliser.
    /// </summary>
    /// <param name="encoding">L'encodage voulu.</param>
    void SetEncoding(Encoding encoding);

    /// <summary>
    /// Fixe le(les) caractère(s) utilisés pour marquer les fins de ligne.
    /// </summary>
    /// <param name="newLine">Les fins de ligne à utiliser.</param>
    void SetNewLine(string newLine);
}