using Luthetus.Common.RazorLib.Exceptions;

namespace Luthetus.TextEditor.RazorLib.Exceptions;

public class LuthetusTextEditorException : LuthetusException
{
    public LuthetusTextEditorException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {

    }
}
