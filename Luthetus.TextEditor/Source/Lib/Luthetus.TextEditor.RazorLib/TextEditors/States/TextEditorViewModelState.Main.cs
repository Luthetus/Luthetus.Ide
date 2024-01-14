using Fluxor;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

/// <summary>
/// Keep the <see cref="TextEditorViewModelState"/> as a class
/// as to avoid record value comparisons when Fluxor checks
/// if the <see cref="FeatureStateAttribute"/> has been replaced.
/// </summary>
[FeatureState]
public partial class TextEditorViewModelState
{
    public TextEditorViewModelState()
    {
        ViewModelList = ImmutableList<TextEditorViewModel>.Empty;
    }

    public ImmutableList<TextEditorViewModel> ViewModelList { get; init; }
}