using System.Collections.Immutable;
using Fluxor;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.Edits.States;

[FeatureState]
public partial record DirtyResourceUriState(ImmutableList<ResourceUri> DirtyResourceUriList)
{
    public DirtyResourceUriState() : this(ImmutableList<ResourceUri>.Empty)
    {
    }
}
