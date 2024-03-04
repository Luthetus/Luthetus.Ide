using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Edits.States;

[FeatureState]
public partial record DirtyResourceUriState(ImmutableList<ResourceUri> DirtyResourceUriList)
{
    public DirtyResourceUriState() : this(ImmutableList<ResourceUri>.Empty)
    {
    }
}
