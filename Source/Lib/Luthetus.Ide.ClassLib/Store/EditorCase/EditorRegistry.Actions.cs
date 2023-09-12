using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.TextEditor.RazorLib.Group;

namespace Luthetus.Ide.ClassLib.Store.EditorCase;

public partial class EditorRegistry
{
    public record ShowInputFileAction();
    public record OpenInEditorAction(IAbsolutePath? AbsoluteFilePath, bool ShouldSetFocusToEditor, TextEditorGroupKey? EditorTextEditorGroupKey = null);
}