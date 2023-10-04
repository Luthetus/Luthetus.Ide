namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// TODO: (2023-06-09) This class is a hacky way to track a cancellation token source per each text editor model. As of this comment, TextEditorModel is a class which is frequently re-constructed with new(). A CancellationTokenSource as a property would therefore not work. One would Cancel() then new() up another CancellationTokenSource and update the property. Yet its possible that instance of TextEditorModel isn't even being used anymore by that point. So a readonly object wrapper which then contains the CancellationTokenSource ensures it doesn't get lost.
/// </summary>
public class TextEditorSaveFileHelper
{
    private object _saveLock = new();
    private CancellationTokenSource _saveCancellationTokenSource = new();

    public CancellationToken GetCancellationToken()
    {
        lock (_saveLock)
        {
            _saveCancellationTokenSource.Cancel();
            _saveCancellationTokenSource = new();

            return _saveCancellationTokenSource.Token;
        }
    }
}