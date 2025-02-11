using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Diffs.Models;

namespace Luthetus.TextEditor.RazorLib.Diffs.Models;

public record struct TextEditorDiffState
{
    public TextEditorDiffState()
    {
        DiffModelList = ImmutableList<TextEditorDiffModel>.Empty;
    }

    public ImmutableList<TextEditorDiffModel> DiffModelList { get; init; }
}