using Fluxor;
using Luthetus.Ide.RazorLib.FindAlls.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.FindAlls.States;

[FeatureState]
public partial record FindAllState(
    string Query,
    string? StartingAbsolutePathForSearch,
    FindAllFilterKind FindAllFilterKind,
    ImmutableList<string> ResultList)
{
    public FindAllState()
        : this(string.Empty, null, FindAllFilterKind.None, ImmutableList<string>.Empty)
    {
    }
}
