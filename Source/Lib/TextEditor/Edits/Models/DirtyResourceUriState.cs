using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public record struct DirtyResourceUriState(List<ResourceUri> DirtyResourceUriList)
{
    public DirtyResourceUriState() : this(new())
    {
    }
}
