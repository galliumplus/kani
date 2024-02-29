namespace Kanmi;

public class PiccUidTests
{
    [Fact]
    public void Parse()
    {
        var validUidSize4 = PiccUid.Parse("01234567");
        Assert.Equal(4, validUidSize4.Size);
        Assert.Equal("01234567", validUidSize4.AsString);
        Assert.Equal(new byte[] { 0x01, 0x23, 0x45, 0x67 }, validUidSize4.AsBytes);

        var validUidSize7 = PiccUid.Parse("01234567-89abcd");
        Assert.Equal(7, validUidSize7.Size);
        Assert.Equal("01234567-89ABCD", validUidSize7.AsString);
        Assert.Equal(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd }, validUidSize7.AsBytes);

        var validUidSize10 = PiccUid.Parse("01234567-89abcd-ef0123");
        Assert.Equal(10, validUidSize10.Size);
        Assert.Equal("01234567-89ABCD-EF0123", validUidSize10.AsString);
        Assert.Equal(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef, 0x01, 0x23 }, validUidSize10.AsBytes);
        
        // trop court
        Assert.Throws<BadUidFormatException>(() => PiccUid.Parse("0123"));
        // trop long
        Assert.Throws<BadUidFormatException>(() => PiccUid.Parse("01234567-89abcd-ef01234567"));
        // tiret manquant
        Assert.Throws<BadUidFormatException>(() => PiccUid.Parse("0123456789abcd"));
        // tiret mal placé
        Assert.Throws<BadUidFormatException>(() => PiccUid.Parse("0123456-789abcd"));
    }

    [Fact]
    public void EqualsAndGetHashCode()
    {
        var lowercaseUid = PiccUid.Parse("0123abcd");
        var uppercaseUid = PiccUid.Parse("0123ABCD");
        Assert.Equal(lowercaseUid, uppercaseUid);
        Assert.Equal(lowercaseUid.GetHashCode(), uppercaseUid.GetHashCode());
        
        var differentUid = PiccUid.Parse("0123ABCD-FFFFFF");
        Assert.NotEqual(lowercaseUid, differentUid);
        Assert.NotEqual(lowercaseUid.GetHashCode(), differentUid.GetHashCode());
    }

    [Fact]
    public void RoundTrip()
    {
        var originalUid = PiccUid.Parse("0123abcd");
        Assert.Equal(originalUid, PiccUid.Parse(originalUid.AsString));
    }
}