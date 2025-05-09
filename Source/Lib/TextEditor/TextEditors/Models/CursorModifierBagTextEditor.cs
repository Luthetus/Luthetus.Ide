using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// Use 'Key<TextEditorViewModel>.Empty'
/// to make a non-user-cursor edit.
/// (a cursor that the user doesn't know exists)
/// </summary>
public struct CursorModifierBagTextEditor
{
    public CursorModifierBagTextEditor(
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCursorModifier cursorModifier)
    {
        CursorModifier = cursorModifier;
        ViewModelKey = viewModelKey;
    }

    /// <summary>
    /// The Key of the view model on which the cursors are being rendered.
    /// </summary>
    public Key<TextEditorViewModel> ViewModelKey;
    public TextEditorCursorModifier CursorModifier;
    public bool ConstructorWasInvoked => CursorModifier is not null;
}
