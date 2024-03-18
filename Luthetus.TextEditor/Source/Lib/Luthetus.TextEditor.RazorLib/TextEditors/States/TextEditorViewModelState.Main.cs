using Fluxor;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

[FeatureState]
public partial class TextEditorViewModelState
{
    public TextEditorViewModelState()
    {
        ViewModelList = ImmutableList<TextEditorViewModel>.Empty;
    }

    public ImmutableList<TextEditorViewModel> ViewModelList { get; init; }
}