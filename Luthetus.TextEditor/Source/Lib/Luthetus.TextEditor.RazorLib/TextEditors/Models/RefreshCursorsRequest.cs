using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial class TextEditorService
{
    public class RefreshCursorsRequest
    {
        public RefreshCursorsRequest(
            Key<TextEditorViewModel> viewModelKey,
            List<TextEditorCursorModifier> cursorBag)
        {
            CursorBag = cursorBag;
            ViewModelKey = viewModelKey;
        }

        /// <summary>
        /// The Key of the view model on which the cursors are being rendered.
        /// </summary>
        public Key<TextEditorViewModel> ViewModelKey { get; }
        public List<TextEditorCursorModifier> CursorBag { get; }
    }
}