using System.Text;
using System.Text.RegularExpressions;
using Kanmi.Exceptions;

namespace Kanmi;

/// <summary>
/// L'identifiant unique d'une carte NFC.
/// </summary>
public partial class PiccUid
{
    private static readonly int[] allowedSizes = { 4, 7, 10 };
        
    private readonly byte[] raw;

    private PiccUid(byte[] raw)
    {
        if (!allowedSizes.Contains(raw.Length))
        {
            throw new BadUidSizeException(raw.Length);
        }
        this.raw = raw;
    }

    // XXXXXXXX, XXXXXXXX-XXXXXX ou XXXXXXXX-XXXXXX-XXXXXX
    // ou les X sont des chiffres hexadécimaux
    [GeneratedRegex("^([0-9a-z]{8})(-([0-9a-z]{6})){0,2}$", RegexOptions.IgnoreCase)]
    private static partial Regex UidRegex();

    private static void ExtractBytesFrom(string text, List<byte> output)
    {
        for (int i = 0; i < text.Length; i += 2)
        {
            output.Add(Convert.ToByte(text.Substring(i, 2), 16));
        }
    }

    private static void DumpBytesTo(byte[] data, StringBuilder output, int from, int to)
    {
        for (int i = from; i < to; i++)
        {
            output.Append(data[i].ToString("X2"));
        }
    }

    /// <summary>
    /// Crée un <see cref="PiccUid"/> depuis sa représentation en texte. Les formats acceptés sont :
    /// <ul>
    ///   <li>XXXXXXXX (8 chiffres, UID de taille 4)</li>
    ///   <li>XXXXXXXX-XXXXXX (14 chiffres, UID de taille 7)</li>
    ///   <li>XXXXXXXX-XXXXXX-XXXXXX (20 chiffres, UID de taille 10)</li>
    /// </ul>
    /// Les chiffres doivent être hexadécimaux.
    /// </summary>
    /// <param name="uid">L'identifiant à convertir.</param>
    /// <returns>Une nouvelle instance d'UID.</returns>
    /// <exception cref="BadUidFormatException">Si le format n'est pas valide.</exception>
    public static PiccUid Parse(string uid)
    {
        Match match = UidRegex().Match(uid);

        if (!match.Success)
        {
            throw new BadUidFormatException(uid);
        }

        var bytes = new List<byte>();

        ExtractBytesFrom(match.Groups[1].Value, bytes);
        foreach (var lastGroupCapture in match.Groups[3].Captures)
        {
            if (lastGroupCapture is Capture capture) ExtractBytesFrom(capture.Value, bytes);
            else if (lastGroupCapture is Group group) ExtractBytesFrom(group.Value, bytes);
        }

        return new PiccUid(bytes.ToArray());
    }
    
    /// <summary>
    /// Cet UID sous forme d'un tableau d'octets.
    /// </summary>
    public byte[] AsBytes => this.raw;

    /// <summary>
    /// La taille de cet UID (4, 7 ou 10).
    /// </summary>
    public int Size => this.raw.Length;

    /// <summary>
    /// Cet UID sous forme de texte, au même format que celui demandé par la méthode <see cref="Parse"/>.
    /// </summary>
    public string AsString
    {
        get
        {
            var uidBuilder = new StringBuilder();
            DumpBytesTo(this.raw, uidBuilder, 0, 4);
            if (this.Size >= 7)
            {
                uidBuilder.Append('-');
                DumpBytesTo(this.raw, uidBuilder, 4, 7);
            }

            if (this.Size == 10)
            {
                uidBuilder.Append('-');
                DumpBytesTo(this.raw, uidBuilder, 7, 10);
            }
            return uidBuilder.ToString();
        }
    }

    public override string ToString()
    {
        return $"{this.GetType().FullName}({this.AsString})";
    }

    private bool Equals(PiccUid other)
    {
        return this.raw.SequenceEqual(other.raw);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj is PiccUid uid && this.Equals(uid);
    }

    public override int GetHashCode()
    {
        int running = 0;
        
        foreach (byte b in this.raw)
        {
            running = HashCode.Combine(running, b);
        }

        return running;
    }
}