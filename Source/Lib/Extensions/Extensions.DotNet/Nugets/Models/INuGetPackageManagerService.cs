using System.Collections.Immutable;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Extensions.DotNet.Nugets.Models;

namespace Luthetus.Extensions.DotNet.Nugets.Models;

public interface INuGetPackageManagerService
{
	public event Action? NuGetPackageManagerStateChanged;
	
	public NuGetPackageManagerState GetNuGetPackageManagerState();
	
    public void ReduceSetSelectedProjectToModifyAction(IDotNetProject? selectedProjectToModify);
    public void ReduceSetNugetQueryAction(string nugetQuery);
    public void ReduceSetIncludePrereleaseAction(bool includePrerelease);
    public void ReduceSetMostRecentQueryResultAction(ImmutableArray<NugetPackageRecord> queryResultList);
}
