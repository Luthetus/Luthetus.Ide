using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.TextEditor.RazorLib.Group;

namespace Luthetus.Ide.RazorLib.EditorCase.States;

public partial class EditorState
{
    public record ShowInputFileAction(EditorSync Sync);
    public record OpenInEditorAction(EditorSync Sync, IAbsolutePath? AbsolutePath, bool ShouldSetFocusToEditor, TextEditorGroupKey? EditorTextEditorGroupKey = null);
}