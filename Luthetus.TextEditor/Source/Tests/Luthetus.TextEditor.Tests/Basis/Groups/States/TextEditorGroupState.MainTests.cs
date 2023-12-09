using Fluxor;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Groups.States;

/// <summary>
/// Keep the <see cref="TextEditorGroupState"/> as a class
/// as to avoid record value comparisons when Fluxor checks
/// if the <see cref="FeatureStateAttribute"/> has been replaced.
/// </summary>
[FeatureState]
public partial class TextEditorGroupState
{
    private TextEditorGroupState()
    {
        GroupBag = ImmutableList<TextEditorGroup>.Empty;
    }

    public ImmutableList<TextEditorGroup> GroupBag { get; init; }
}