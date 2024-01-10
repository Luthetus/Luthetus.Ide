using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public class TextEditorCursorModifierBag
{
    public TextEditorCursorModifierBag(
        Key<TextEditorViewModel> viewModelKey,
        List<TextEditorCursorModifier> cursorModifierList)
    {
        List = cursorModifierList;
        ViewModelKey = viewModelKey;
    }

    /// <summary>
    /// The Key of the view model on which the cursors are being rendered.
    /// </summary>
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public List<TextEditorCursorModifier> List { get; }
}