using Fluxor;
using Luthetus.Ide.RazorLib.CodeSearches.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.CodeSearches.States;

[FeatureState]
public partial record CodeSearchState(
    string Query,
    string? StartingAbsolutePathForSearch,
    CodeSearchFilterKind CodeSearchFilterKind,
    ImmutableList<string> ResultList)
{
    public CodeSearchState()
        : this(string.Empty, null, CodeSearchFilterKind.None, ImmutableList<string>.Empty)
    {
    }
}
