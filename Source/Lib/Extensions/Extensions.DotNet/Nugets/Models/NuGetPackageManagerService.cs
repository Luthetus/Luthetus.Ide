using System.Collections.Immutable;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;

namespace Luthetus.Extensions.DotNet.Nugets.Models;

public class NuGetPackageManagerService : INuGetPackageManagerService
{
	private NuGetPackageManagerState _nuGetPackageManagerState = new();
	
	public event Action? NuGetPackageManagerStateChanged;
	
	public NuGetPackageManagerState GetNuGetPackageManagerState() => _nuGetPackageManagerState;
	
    public void ReduceSetSelectedProjectToModifyAction(IDotNetProject? selectedProjectToModify)
    {
    	var inState = GetNuGetPackageManagerState();
    
        _nuGetPackageManagerState = inState with
        {
            SelectedProjectToModify = selectedProjectToModify
        };
        
        NuGetPackageManagerStateChanged?.Invoke();
        return;
    }

    public void ReduceSetNugetQueryAction(string nugetQuery)
    {
    	var inState = GetNuGetPackageManagerState();
    
        _nuGetPackageManagerState = inState with { NugetQuery = nugetQuery };
        
        NuGetPackageManagerStateChanged?.Invoke();
        return;
    }

    public void ReduceSetIncludePrereleaseAction(bool includePrerelease)
    {
    	var inState = GetNuGetPackageManagerState();
    
        _nuGetPackageManagerState = inState with { IncludePrerelease = includePrerelease };
        
        NuGetPackageManagerStateChanged?.Invoke();
        return;
    }

    public void ReduceSetMostRecentQueryResultAction(ImmutableArray<NugetPackageRecord> queryResultList)
    {
    	var inState = GetNuGetPackageManagerState();
    
        _nuGetPackageManagerState = inState with { QueryResultList = queryResultList };
        
        NuGetPackageManagerStateChanged?.Invoke();
        return;
    }
}
