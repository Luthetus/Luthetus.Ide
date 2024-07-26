using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.Edits.States;

public partial record DirtyResourceUriState
{
    public record AddDirtyResourceUriAction(ResourceUri ResourceUri);
    public record RemoveDirtyResourceUriAction(ResourceUri ResourceUri);
}
