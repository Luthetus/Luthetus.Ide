using Luthetus.Ide.ClassLib.DotNet;
using Luthetus.Ide.ClassLib.Nuget;

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