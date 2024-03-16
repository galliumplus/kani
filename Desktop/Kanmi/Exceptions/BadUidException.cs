namespace Kanmi.Exceptions;

/// <summary>
/// La base des exceptions qui peuvent être levées en utilisant des <see cref="PiccUid"/>.
/// </summary>
public abstract class BadUidException: KanmiException
{
    /// <inheritdoc />
    protected BadUidException(string message) : base(message)
    {
    }
}