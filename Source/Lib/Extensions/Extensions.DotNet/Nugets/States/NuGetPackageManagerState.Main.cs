using System.Collections.Immutable;
using Fluxor;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Extensions.DotNet.Nugets.Models;

namespace Luthetus.Extensions.DotNet.Nugets.States;

[FeatureState]
public partial record NuGetPackageManagerState(
    IDotNetProject? SelectedProjectToModify,
    string NugetQuery,
    bool IncludePrerelease,
    ImmutableArray<NugetPackageRecord> QueryResultList)
{
    public NuGetPackageManagerState() : this(null, string.Empty, false, ImmutableArray<NugetPackageRecord>.Empty)
    {

    }
}