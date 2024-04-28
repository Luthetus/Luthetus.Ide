using Luthetus.Common.RazorLib.Exceptions;

namespace Luthetus.Ide.RazorLib.Exceptions;

public class LuthetusIdeException : LuthetusException
{
    public LuthetusIdeException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {

    }
}
