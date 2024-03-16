using System.Diagnostics;
using Kanmi.Protocols;
using Kanmi.Serial;

namespace Kanmi.ServicesImplementation;

internal abstract class BaseService : ICardReaderService
{
    private readonly ISerialDiscovery serial;
    private readonly List<ICardReaderListener> listeners;
    private bool isActive;
    private ISerialPort? currentReader;

    protected BaseService(ISerialDiscovery serial)
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

    public virtual void Start()
    {
        this.isActive = true;
    }

    public virtual void Stop()
    {
        this.isActive = false;
    }

    protected void NotifyReaderConnected(string readerInfos)
    {
        foreach (ICardReaderListener listener in this.listeners) listener.OnReaderConnected(readerInfos);
    }

    protected void NotifyReaderDisconnected()
    {
        foreach (ICardReaderListener listener in this.listeners) listener.OnReaderDisconnected();
    }

    protected void NotifyEngagedWithPicc(PiccUid uid)
    {
        foreach (ICardReaderListener listener in this.listeners) listener.OnEngagedWithPicc(uid);
    }

    protected void NotifyUnexpectedError(ErrorContext error)
    {
        foreach (ICardReaderListener listener in this.listeners) listener.OnUnexpectedError(error);
    }

    protected ISerialPort? CurrentReader => this.currentReader;

    protected void UseReader(ISerialPort port)
    {
        this.currentReader = port;
    }

    protected void DisposeOfReader()
    {
        this.currentReader?.EnsureIsClosed();
        this.currentReader = null;
    }

    protected async Task DisposeOfReaderAsync(CancellationToken ct = default)
    {
        if (this.currentReader != null)
        {
            await this.currentReader.EnsureIsClosedAsync(ct);
            this.currentReader = null;
        }
    }

    protected IEnumerable<ISerialPort> ListNewlyAvailablePorts()
    {
        return this.serial.ListNewlyAvailablePorts();
    }
}