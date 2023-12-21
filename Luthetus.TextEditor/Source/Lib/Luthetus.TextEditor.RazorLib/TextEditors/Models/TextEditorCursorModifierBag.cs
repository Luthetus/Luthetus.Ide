using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using System.Collections;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public class TextEditorCursorModifierBag : IEnumerable
{
    public TextEditorCursorModifierBag(
        Key<TextEditorViewModel> viewModelKey,
        List<TextEditorCursorModifier> cursorModifierBag)
    {
        CursorModifierBag = cursorModifierBag;
        ViewModelKey = viewModelKey;
    }

    /// <summary>
    /// The Key of the view model on which the cursors are being rendered.
    /// </summary>
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public List<TextEditorCursorModifier> CursorModifierBag { get; }

    public IEnumerator GetEnumerator()
    {
        return CursorModifierBag.GetEnumerator();
    }
}