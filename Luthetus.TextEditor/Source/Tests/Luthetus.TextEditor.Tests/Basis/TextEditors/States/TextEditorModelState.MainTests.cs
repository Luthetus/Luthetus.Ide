using Fluxor;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

/// <summary>
/// Keep the <see cref="TextEditorModelState"/> as a class
/// as to avoid record value comparisons when Fluxor checks
/// if the <see cref="FeatureStateAttribute"/> has been replaced.
/// </summary>
[FeatureState]
public partial class TextEditorModelStateTests
{
    private TextEditorModelState()
    {
        ModelBag = ImmutableList<TextEditorModel>.Empty;
    }

    public ImmutableList<TextEditorModel> ModelBag { get; init; }
}