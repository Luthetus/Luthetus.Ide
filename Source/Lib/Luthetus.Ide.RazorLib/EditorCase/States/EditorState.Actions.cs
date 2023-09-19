using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;
using Luthetus.TextEditor.RazorLib.Group;
using Luthetus.TextEditor.RazorLib.Group.Models;

namespace Luthetus.Ide.RazorLib.EditorCase.States;

public partial class EditorState
{
    public record ShowInputFileAction(EditorSync Sync);
    public record OpenInEditorAction(EditorSync Sync, IAbsolutePath? AbsolutePath, bool ShouldSetFocusToEditor, Key<TextEditorGroup>? EditorTextEditorGroupKey = null);
}