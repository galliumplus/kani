using Kanmi.Protocols;
using Kanmi.Serial;

namespace Kanmi.ServicesImplementation;

internal class BlockingService: BaseService
{
    public BlockingService(ISerialDiscovery serial) : base(serial)
    {
    }
    
    public override void Start()
    {
        base.Start();
        
        while (this.IsActive)
        {
            this.Loop();
        }
    }

    private void Loop()
    {
        try
        {
            if (this.CurrentReader == null)
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
        foreach (ISerialPort port in this.ListNewlyAvailablePorts())
        {
            if (port.MayBeACardReader && port.TryToConnect())
            {
                this.UseReader(port);
                this.NotifyReaderConnected(this.CurrentReader?.ReaderInfos ?? "<inconnu>");
                break;
            }
        }
    }

    private void ReadMessages()
    {
        if (this.CurrentReader != null)
        {
            try
            {
                Message message = this.CurrentReader.ReadNextMessage();

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