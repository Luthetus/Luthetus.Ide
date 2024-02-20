using Fluxor;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.Nugets.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.Tests.Basis.Nugets.States;

public class NuGetPackageManagerStateMainTests(
    IDotNetProject? SelectedProjectToModify,
    string NugetQuery,
    bool IncludePrerelease,
    ImmutableArray<NugetPackageRecord> QueryResultList)
{
    public NuGetPackageManagerState() : this(null, string.Empty, false, ImmutableArray<NugetPackageRecord>.Empty)
    {

    }
}