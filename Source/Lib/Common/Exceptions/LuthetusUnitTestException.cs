namespace Luthetus.Common.RazorLib.Exceptions;

public class LuthetusUnitTestException : LuthetusException
{
    public LuthetusUnitTestException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {

    }
}
