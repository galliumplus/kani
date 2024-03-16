using System.IO.Ports;
using System.Text;
using Kanmi.Protocols;

namespace Kanmi.Serial.NetCore;

/// <summary>
/// Une implémentation de <see cref="ISerialPort"/> basée sur la classe multiplateforme <see cref="SerialPort"/>.
/// </summary>
public class NetCoreSerialPort : ISerialPort, IConfigurableSerialPort
{
    private const int READ_TIMEOUT = 3000; // 3s

    private static readonly ushort?[] knownVendors =
    {
        0x2341, 0x2A03, 0x1B4F, 0x239A, // Arduino AVR standard
        0x1A86, // Vendeur alternatif utilisé dans le G+KR1
    };

    private readonly SerialPort underlying;
    private readonly ushort? vendor;
    private bool lostConnection;
    private string? currentReaderInfos;
    private IProtocol? protocol;

    /// <summary>
    /// Crée un port série multiplateforme.
    /// </summary>
    /// <param name="name">L'identifiant du port.</param>
    /// <param name="vendor">L'identifiant du fabricant de l'appareil.</param>
    public NetCoreSerialPort(string name, ushort? vendor)
    {
        this.underlying = new SerialPort(name);
        this.underlying.ReadTimeout = READ_TIMEOUT;

        this.lostConnection = false;
        this.vendor = vendor;
        this.currentReaderInfos = null;
    }

    /// <inheritdoc />
    // Identifiants fournisseurs des Arduino AVR
    public bool MayBeACardReader => knownVendors.Contains(this.vendor);

    /// <inheritdoc />
    public bool StillAvailable => !this.lostConnection;

    /// <inheritdoc />
    public string? ReaderInfos => this.currentReaderInfos;

    /// <inheritdoc />
    public void SetBaudRate(int rate) => this.underlying.BaudRate = rate;

    /// <inheritdoc />
    public void SetEncoding(Encoding encoding) => this.underlying.Encoding = encoding;

    /// <inheritdoc />
    public void SetNewLine(string newLine) => this.underlying.NewLine = newLine;

    /// <inheritdoc />
    public bool TryToConnect()
    {
        bool connected = false;

        try
        {
            foreach (IProtocol protocolToTry in AllProtocols.ProtocolsList)
            {
                connected = this.TryToConnectUsing(protocolToTry);
                if (connected) break;
            }
        }
        catch (Exception error)
        {
            if (MeansConnectionWasClosed(error))
            {
                connected = false;
                this.lostConnection = true;
            }
            else throw;
        }

        if (!connected)
        {
            // si toutes les tentatives de connection ont échouées, on ferme le port
            this.EnsureIsClosed();
        }

        return connected;
    }

    /// <inheritdoc />
    public async Task<bool> TryToConnectAsync(CancellationToken ct = default)
    {
        bool connected = false;

        try
        {
            foreach (IProtocol protocolToTry in AllProtocols.ProtocolsList)
            {
                ct.ThrowIfCancellationRequested();
                connected = this.TryToConnectUsing(protocolToTry, ct);
                if (connected) break;
            }
        }
        catch (Exception error)
        {
            if (MeansConnectionWasClosed(error))
            {
                connected = false;
                this.lostConnection = true;
            }
            else throw;
        }

        if (!connected)
        {
            // si toutes les tentatives de connection ont échouées, on ferme le port
            await this.EnsureIsClosedAsync(ct);
        }

        return connected;
    }

    /// <inheritdoc />
    public void EnsureIsClosed()
    {
        if (this.underlying.IsOpen)
        {
            this.underlying.Close();
        }
    }

    /// <inheritdoc />
    public Task EnsureIsClosedAsync(CancellationToken ct = default)
    {
        if (this.underlying.IsOpen)
        {
            this.underlying.Close();
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Message ReadNextMessage()
    {
        return this.ReadNextMessageImpl();
    }

    /// <inheritdoc />
    public Task<Message> ReadNextMessageAsync(CancellationToken ct = default)
    {
        return Task.FromResult(this.ReadNextMessageImpl(ct));
    }

    private Message ReadNextMessageImpl(CancellationToken ct = default)
    {
        if (this.protocol == null) throw new InvalidOperationException("Aucune connexion n'a été établie.");

        Message result;

        try
        {
            string rawMessage = this.underlying.ReadLine();
            ct.ThrowIfCancellationRequested();
            result = this.protocol.ParseMessage(rawMessage);
        }
        catch (Exception error)
        {
            if (MeansConnectionWasClosed(error))
            {
                // l'appareil a été déconnecté pendant la réception du message
                result = new Message.ConnectionEnded();
                this.lostConnection = true;
            }
            else throw;
        }

        return result;
    }

    private bool TryToConnectUsing(IProtocol protocolToTry, CancellationToken ct = default)
    {
        bool success = false;

        protocolToTry.ConfigurePort(this);
        try
        {
            if (this.underlying.IsOpen)
            {
                this.ResetDevice();
            }
            else
            {
                this.underlying.Open();
                ct.ThrowIfCancellationRequested();
            }

            string firstMessage = this.underlying.ReadLine();
            ct.ThrowIfCancellationRequested();
            if (protocolToTry.CanHandle(firstMessage))
            {
                this.protocol = protocolToTry;
                this.currentReaderInfos =
                    $"{this.protocol.ExtractReaderInfos(firstMessage)} sur {this.underlying.PortName}";
                success = true;
            }
        }
        catch (UnauthorizedAccessException)
        {
            // le port est déjà ouvert par un autre programme
        }
        catch (InvalidOperationException)
        {
            // le port est déjà ouvert par cette classe
        }

        return success;
    }

    private void ResetDevice()
    {
        this.underlying.DtrEnable = true;
        Thread.Sleep(50);
        this.underlying.DtrEnable = false;
    }

    private static bool MeansConnectionWasClosed(Exception error)
    {
        return error is OperationCanceledException or FileNotFoundException or UnauthorizedAccessException;
    }
}