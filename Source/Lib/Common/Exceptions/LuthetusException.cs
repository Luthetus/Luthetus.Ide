namespace Luthetus.Common.RazorLib.Exceptions;

public abstract class LuthetusException : Exception
{
    public LuthetusException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
        
    }
}
