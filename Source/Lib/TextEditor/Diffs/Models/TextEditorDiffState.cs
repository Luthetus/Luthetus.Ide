namespace Luthetus.TextEditor.RazorLib.Diffs.Models;

public record struct TextEditorDiffState
{
    public TextEditorDiffState()
    {
        DiffModelList = Array.Empty<TextEditorDiffModel>();
    }

    public IReadOnlyList<TextEditorDiffModel> DiffModelList { get; init; }
}