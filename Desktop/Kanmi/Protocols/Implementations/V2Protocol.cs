using System.Text;
using System.Text.RegularExpressions;
using Kanmi.Serial;

namespace Kanmi.Protocols.Implementations;

/// <summary>
/// Implémentation du protocole Kanmi v2.0
/// </summary>
internal class V2Protocol : IProtocol
{
    public string Name => "Kanmi 2.0";

    public void ConfigurePort(IConfigurableSerialPort port)
    {
        port.SetBaudRate(115_200);
        port.SetEncoding(new ASCIIEncoding());
        port.SetNewLine("\r\n");
    }

    public bool CanHandle(string firstMessage)
    {
        var message = V2Message.Parse(firstMessage);
        return message.Kind == V2Message.MessageKind.ReaderVersion && message.Argument(0) == "02";
    }

    public string ExtractReaderInfos(string firstMessage)
    {
        var message = V2Message.Parse(firstMessage);
        var result = new StringBuilder();

        if (message.Kind == V2Message.MessageKind.ReaderVersion)
        {
            result.Append("Lecteur Kanmi v2.0");

            switch (message.Argument(1))
            {
            case "88":
                result.Append(" (type FM17522)");
                break;
            case "90":
                result.Append(" (MFRC522 v0.0)");
                break;
            case "91":
                result.Append(" (MFRC522 v1.0)");
                break;
            case "92":
                result.Append(" (MFRC522 v2.0)");
                break;
            }
        }

        return result.ToString();
    }

    public Message ParseMessage(string message)
    {
        var parsedMessage = V2Message.Parse(message);

        Message result;
        switch (parsedMessage.Kind)
        {
            case V2Message.MessageKind.EngagedWithPicc:
                var uid = PiccUid.Parse(parsedMessage.Argument(0));
                result = new Message.EngagedWithPiccMessage(uid);
                break;
            
            default:
                result = new Message.UnknownIntent();
                break;
        }

        return result;
    }
}