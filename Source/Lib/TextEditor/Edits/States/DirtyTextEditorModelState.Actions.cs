using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.Edits.States;

public partial record DirtyResourceUriState
{
    public record AddDirtyResourceUriAction(ResourceUri ResourceUri);
    public record RemoveDirtyResourceUriAction(ResourceUri ResourceUri);
}
