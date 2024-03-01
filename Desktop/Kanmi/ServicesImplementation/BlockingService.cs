using System.Diagnostics;
using Kanmi.Protocols;
using Kanmi.Serial;

namespace Kanmi.ServicesImplementation;

internal class BlockingService : ICardReaderService
{
    private readonly ISerialDiscovery serial;
    private readonly List<ICardReaderListener> listeners;
    private bool isActive;
    private ISerialPort? currentReader;

    public BlockingService(ISerialDiscovery serial)
    {
        this.serial = serial;
        this.listeners = new List<ICardReaderListener>();
        
        this.isActive = false;
        this.DisposeOfReader();
    }

    public bool IsActive => this.isActive;

    public void Subscribe(ICardReaderListener listener)
    {
        this.listeners.Add(listener);
    }

    public void Unsubscribe(ICardReaderListener listener)
    {
        this.listeners.Remove(listener);
    }

    public void Start()
    {
        this.isActive = true;
        while (this.isActive)
        {
            this.Loop();
        }
    }

    public void Stop()
    {
        this.isActive = false;
    }

    private void NotifyReaderConnected(string readerInfos)
    {
        foreach (ICardReaderListener listener in this.listeners) listener.OnReaderConnected(readerInfos);
    }

    private void NotifyReaderDisconnected()
    {
        foreach (ICardReaderListener listener in this.listeners) listener.OnReaderDisconnected();
    }

    private void NotifyEngagedWithPicc(PiccUid uid)
    {
        foreach (ICardReaderListener listener in this.listeners) listener.OnEngagedWithPicc(uid);
    }

    private void NotifyUnexpectedError(ErrorContext error)
    {
        foreach (ICardReaderListener listener in this.listeners) listener.OnUnexpectedError(error);
    }

    private void Loop()
    {
        try
        {
            if (this.currentReader == null)
            {
                this.TryToConnect();
                Thread.Sleep(1000);
            }
            else
            {
                this.ReadMessages();
            }
        }
        catch (Exception unexpectedError)
        {
            var ctx = new ErrorContext(unexpectedError);
            this.NotifyUnexpectedError(ctx);

            if (ctx.WasHandled)
            {
                // redémarrage
                this.DisposeOfReader();
            }
            else throw;
        }
    }

    private void TryToConnect()
    {
        foreach (ISerialPort port in this.serial.ListNewlyAvailablePorts())
        {
            if (port.MayBeACardReader && port.TryToConnect())
            {
                this.currentReader = port;
                this.NotifyReaderConnected(this.currentReader.ReaderInfos ?? "<inconnu>");
                break;
            }
        }
    }

    private void ReadMessages()
    {
        if (this.currentReader != null)
        {
            try
            {
                Message message = this.currentReader.ReadNextMessage();

                switch (message)
                {
                case Message.EngagedWithPiccMessage { Uid: var uid }:
                    this.NotifyEngagedWithPicc(uid);
                    break;

                case Message.ConnectionEnded:
                    this.NotifyReaderDisconnected();
                    this.DisposeOfReader();
                    break;
                }
            }
            catch (TimeoutException)
            {
                // on réessaie à la prochaine boucle
            }
        }
    }

    private void DisposeOfReader()
    {
        this.currentReader?.EnsureIsClosed();
        this.currentReader = null;
    }
}