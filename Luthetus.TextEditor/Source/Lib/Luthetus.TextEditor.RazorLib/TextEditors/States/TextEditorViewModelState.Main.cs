using Luthetus.TextEditor.RazorLib.PolymorphicUis.Models;
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
        ViewModelPolymorphicUiList = ImmutableList<TextEditorViewModelPolymorphicUi>.Empty;
    }

    public ImmutableList<TextEditorViewModel> ViewModelList { get; init; }
    public ImmutableList<TextEditorViewModelPolymorphicUi> ViewModelPolymorphicUiList { get; init; }
}