using Fluxor;
using Luthetus.CompilerServices.Lang.DotNet;
using Luthetus.Ide.ClassLib.Nuget;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Store.NugetPackageManagerCase;

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