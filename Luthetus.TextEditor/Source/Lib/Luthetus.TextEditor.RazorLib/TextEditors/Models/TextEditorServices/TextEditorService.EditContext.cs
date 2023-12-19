using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial class TextEditorService
{
    private class TextEditorEditContext : ITextEditorEditContext
    {
        public TextEditorCommandArgs CommandArgs { get; set; }
        public TextEditorModel Model { get; set; }
        public TextEditorViewModel ViewModel { get; set; }
        public RefreshCursorsRequest RefreshCursorsRequest { get; set; }
        public TextEditorCursorModifier PrimaryCursor { get; set; }
        public bool? IsCompleted { get; private set; }
    }
}

