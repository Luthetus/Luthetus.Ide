using Luthetus.Common.RazorLib.Exceptions;

namespace Luthetus.TextEditor.RazorLib.Exceptions;

/// <remarks>
/// This class is an exception to the naming convention, "don't use the word 'Luthetus' in class names".
/// 
/// Reason for this exception: the 'Exception' datatype is far more common in code,
/// 	than some specific type (example: DialogDisplay.razor).
///     So, adding 'Luthetus' in the class name for redundancy seems meaningful here.
/// </remarks>
public class LuthetusTextEditorException : LuthetusException
{
    public LuthetusTextEditorException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {

    }
}
