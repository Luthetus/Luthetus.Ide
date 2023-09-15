using Fluxor;
using Luthetus.CompilerServices.Lang.DotNetSolution;
using Luthetus.Ide.RazorLib.NugetCase.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.NugetCase.States;

[FeatureState]
public partial record NuGetPackageManagerState(
    IDotNetProject? SelectedProjectToModify,
    string NugetQuery,
    bool IncludePrerelease,
    ImmutableArray<NugetPackageRecord> MostRecentQueryResult)
{
    public NuGetPackageManagerState()
        : this(
            default(IDotNetProject?),
            string.Empty,
            false,
            ImmutableArray<NugetPackageRecord>.Empty)
    {

    }
}