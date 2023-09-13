using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.TextEditor.RazorLib.Group;

namespace Luthetus.Ide.RazorLib.EditorCase;

public partial class EditorRegistry
{
    public record ShowInputFileAction();
    public record OpenInEditorAction(IAbsolutePath? AbsolutePath, bool ShouldSetFocusToEditor, TextEditorGroupKey? EditorTextEditorGroupKey = null);
}