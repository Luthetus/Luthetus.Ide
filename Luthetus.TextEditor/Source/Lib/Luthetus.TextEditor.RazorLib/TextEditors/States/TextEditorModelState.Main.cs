using Fluxor;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

[FeatureState]
public partial class TextEditorModelState
{
    public TextEditorModelState()
    {
        ModelList = ImmutableList<TextEditorModel>.Empty;
    }

    public ImmutableList<TextEditorModel> ModelList { get; init; }
}