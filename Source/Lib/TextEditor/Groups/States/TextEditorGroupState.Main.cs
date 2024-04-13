using Fluxor;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Groups.States;

[FeatureState]
public partial class TextEditorGroupState
{
    public TextEditorGroupState()
    {
        GroupList = ImmutableList<TextEditorGroup>.Empty;
    }

    public ImmutableList<TextEditorGroup> GroupList { get; init; }
}