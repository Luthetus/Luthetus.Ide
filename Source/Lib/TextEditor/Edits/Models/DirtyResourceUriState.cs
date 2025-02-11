using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.Edits.Models;

public record struct DirtyResourceUriState(ImmutableList<ResourceUri> DirtyResourceUriList)
{
    public DirtyResourceUriState() : this(ImmutableList<ResourceUri>.Empty)
    {
    }
}
