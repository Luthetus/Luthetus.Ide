using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.TextEditor.RazorLib.Group;

namespace Luthetus.Ide.ClassLib.Store.EditorCase;

public partial class EditorState
{
    public record ShowInputFileAction();
    public record OpenInEditorAction(IAbsoluteFilePath? AbsoluteFilePath, bool ShouldSetFocusToEditor, TextEditorGroupKey? EditorTextEditorGroupKey = null);
}
