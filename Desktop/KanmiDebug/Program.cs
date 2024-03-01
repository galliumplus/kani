using Kanmi;
using KanmiDebug;

ICardReaderService service = Services.MakeBlockingService();

service.Subscribe(new LogListener());

service.Start();