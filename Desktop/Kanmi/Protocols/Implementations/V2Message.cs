using System.Text.RegularExpressions;

namespace Kanmi.Protocols.Implementations;

internal partial class V2Message
{
    public enum MessageKind { Invalid, ReaderVersion, EngagedWithPicc }
    
    // KANMI:TYPE_DU_MESSAGE ARGUMENT1 ARGUMENT2 ...
    [GeneratedRegex(@"^KANMI:([A-Z_]+)( ([^ ]+))*$")]
    private static partial Regex MessageRegex();

    private readonly MessageKind kind;
    private readonly string[] arguments;

    private V2Message(MessageKind kind, string[] arguments)
    {
        this.kind = kind;
        this.arguments = arguments;
    }

    public bool IsValid => this.kind != MessageKind.Invalid;

    public MessageKind Kind => this.kind;

    public string Argument(int index) => this.arguments[index];

    public static V2Message Parse(string rawMessage)
    {
        Match match = MessageRegex().Match(rawMessage);

        V2Message message = new V2Message(MessageKind.Invalid, Array.Empty<string>());

        if (match.Success)
        {
            List<string> args = new();
            foreach (var argCapture in match.Groups[3].Captures)
            {
                if (argCapture is Capture capture) args.Add(capture.Value);
                else if (argCapture is Group group) args.Add(group.Value);
            }
            
            switch (match.Groups[1].Value)
            {
            case "READER_VERSION" when args.Count == 2:
                message = new V2Message(MessageKind.ReaderVersion, args.ToArray());
                break;
            
            case "ENGAGED_WITH_PICC" when args.Count == 1:
                message = new V2Message(MessageKind.EngagedWithPicc, args.ToArray());
                break;
            }
        }
        
        return message;
    }
}