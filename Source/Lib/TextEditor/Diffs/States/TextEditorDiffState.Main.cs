using Fluxor;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Diffs.States;

[FeatureState]
public partial class TextEditorDiffState
{
    public TextEditorDiffState()
    {
        DiffModelList = ImmutableList<TextEditorDiffModel>.Empty;
    }

    public ImmutableList<TextEditorDiffModel> DiffModelList { get; init; }
}