using Fluxor;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Edits.States;

[FeatureState]
public partial record DirtyResourceUriState(ImmutableList<ResourceUri> DirtyResourceUriList)
{
    public DirtyResourceUriState() : this(ImmutableList<ResourceUri>.Empty)
    {
    }
}
