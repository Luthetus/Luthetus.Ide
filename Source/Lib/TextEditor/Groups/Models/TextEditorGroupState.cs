using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Groups.Models;


namespace Luthetus.TextEditor.RazorLib.Groups.Models;

public partial class TextEditorGroupState
{
    public TextEditorGroupState()
    {
        GroupList = ImmutableList<TextEditorGroup>.Empty;
    }

    public ImmutableList<TextEditorGroup> GroupList { get; init; }
}