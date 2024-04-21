namespace Luthetus.Common.RazorLib.Exceptions;

public class LuthetusCommonException : LuthetusException
{
    public LuthetusCommonException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {

    }
}
