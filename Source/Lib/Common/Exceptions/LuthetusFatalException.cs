namespace Luthetus.Common.RazorLib.Exceptions;

public class LuthetusFatalException : LuthetusException
{
    public LuthetusFatalException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {

    }
}
