using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Diffs.Models;

public record struct TextEditorDiffState
{
    public TextEditorDiffState()
    {
        DiffModelList = ImmutableList<TextEditorDiffModel>.Empty;
    }

    public ImmutableList<TextEditorDiffModel> DiffModelList { get; init; }
}