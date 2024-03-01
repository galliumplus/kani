using System.Text;

namespace Kanmi.Protocols.Implementations;

public class V2MessageTests
{
    [Fact]
    public void Parse()
    {
        var msg1 = V2Message.Parse("KANMI:READER_VERSION 02 92");
        Assert.True(msg1.IsValid);
        Assert.Equal(V2Message.MessageKind.ReaderVersion, msg1.Kind);
        Assert.Equal("02", msg1.Argument(0));
        Assert.Equal("92", msg1.Argument(1));
        
        var msg2 = V2Message.Parse("KANMI:ENGAGED_WITH_PICC 01234567");
        Assert.True(msg2.IsValid);
        Assert.Equal(V2Message.MessageKind.EngagedWithPicc, msg2.Kind);
        Assert.Equal("01234567", msg2.Argument(0));
        
        var msg3 = V2Message.Parse("KANMI:READER_VERSION");
        Assert.False(msg3.IsValid);
        Assert.Equal(V2Message.MessageKind.Invalid, msg3.Kind);
        
        var msg4 = V2Message.Parse("KANMI:ENGAGED_WITH_PICC 0123 4567 89AB");
        Assert.False(msg4.IsValid);
        Assert.Equal(V2Message.MessageKind.Invalid, msg4.Kind);
        
        var msg5 = V2Message.Parse("READER_VERSION 02 92");
        Assert.False(msg5.IsValid);
        Assert.Equal(V2Message.MessageKind.Invalid, msg5.Kind);
    }
}