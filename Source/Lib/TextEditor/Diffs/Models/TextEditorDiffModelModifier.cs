namespace Luthetus.TextEditor.RazorLib.Diffs.Models;

public class TextEditorDiffModelModifier
{
    public TextEditorDiffModelModifier(TextEditorDiffModel diffModel)
    {
        DiffModel = diffModel;
    }

    public TextEditorDiffModel DiffModel { get; set; }
    public bool WasModified { get; internal set; }
}