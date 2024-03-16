using Kanmi.Protocols;
using Kanmi.Serial;

namespace Kanmi.ServicesImplementation;

internal class BackgroundService : BaseService
{
    private readonly object controlLock;
    private CancellationTokenSource? cts;
    private Task? execution;

    public BackgroundService(ISerialDiscovery serial) : base(serial)
    {
        this.controlLock = new object();
        this.cts = null;
        this.execution = null;
    }

    public override void Start()
    {
        lock (this.controlLock)
        {
            if (!this.IsActive)
            {
                base.Start();

                this.cts = new CancellationTokenSource();
                this.execution = this.Run(this.cts.Token);
            }
        }
    }

    public override void Stop()
    {
        lock (this.controlLock)
        {
            if (this.IsActive)
            {
                base.Stop();

                this.cts?.Cancel();
                this.execution?.Wait();
                this.cts?.Dispose();
            }
        }
    }

    private async Task Run(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await this.Loop(ct);
        }
    }

    private async Task Loop(CancellationToken ct)
    {
        try
        {
            if (this.CurrentReader == null)
            {
                await this.TryToConnect(ct);
            }
            else
            {
                await this.ReadMessages(ct);
            }
        }
        catch (OperationCanceledException)
        {
            await this.DisposeOfReaderAsync(ct);
        }
        catch (Exception unexpectedError)
        {
            var ctx = new ErrorContext(unexpectedError);
            this.NotifyUnexpectedError(ctx);

            if (ctx.WasHandled)
            {
                // redémarrage
                await this.DisposeOfReaderAsync(ct);
            }
            else throw;
        }
    }

    private async Task TryToConnect(CancellationToken ct)
    {
        bool connected = false;
        foreach (ISerialPort port in this.ListNewlyAvailablePorts())
        {
            bool shouldTryToConnect = !connected && !ct.IsCancellationRequested && port.MayBeACardReader;
            if (shouldTryToConnect && await port.TryToConnectAsync(ct))
            {
                this.UseReader(port);
                this.NotifyReaderConnected(this.CurrentReader?.ReaderInfos ?? "<inconnu>");
                break;
            }
        }
    }

    private async Task ReadMessages(CancellationToken ct)
    {
        if (this.CurrentReader != null)
        {
            try
            {
                Message message = await this.CurrentReader.ReadNextMessageAsync(ct);

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
}