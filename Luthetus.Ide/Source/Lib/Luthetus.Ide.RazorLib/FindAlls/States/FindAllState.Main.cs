using Fluxor;
using Luthetus.Ide.RazorLib.FindAlls.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.FindAlls.States;

[FeatureState]
public partial record FindAllState(
    string Query,
    FindAllFilterKind FindAllFilterKind,
    ImmutableList<string> ResultList)
{
    public FindAllState()
        : this(string.Empty, FindAllFilterKind.None, ImmutableList<string>.Empty)
    {
    }
}
