using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Groups.Models;

namespace Luthetus.TextEditor.RazorLib.Groups.Models;

public record struct TextEditorGroupState
{
    public TextEditorGroupState()
    {
        GroupList = ImmutableList<TextEditorGroup>.Empty;
    }

    public ImmutableList<TextEditorGroup> GroupList { get; init; }
}