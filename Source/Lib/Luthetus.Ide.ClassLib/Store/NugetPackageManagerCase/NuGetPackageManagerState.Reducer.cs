namespace Luthetus.Ide.ClassLib.Store.NugetPackageManagerCase;

public partial record NuGetPackageManagerState
{
    private class NuGetPackageManagerStateReducer
    {
        [ReducerMethod]
        public static NuGetPackageManagerState ReduceSetSelectedProjectToModifyAction(
            NuGetPackageManagerState inNuGetPackageManagerState,
            SetSelectedProjectToModifyAction setSelectedProjectToModifyAction)
        {
            return inNuGetPackageManagerState with
            {
                SelectedProjectToModify = 
                    setSelectedProjectToModifyAction.SelectedProjectToModify
            };
        }
        
        [ReducerMethod]
        public static NuGetPackageManagerState ReduceSetNugetQueryAction(
            NuGetPackageManagerState inNuGetPackageManagerState,
            SetNugetQueryAction setNugetQueryAction)
        {
            return inNuGetPackageManagerState with
            {
                NugetQuery = 
                    setNugetQueryAction.NugetQuery
            };
        }
        
        [ReducerMethod]
        public static NuGetPackageManagerState ReduceSetIncludePrereleaseAction(
            NuGetPackageManagerState inNuGetPackageManagerState,
            SetIncludePrereleaseAction setIncludePrereleaseAction)
        {
            return inNuGetPackageManagerState with
            {
                IncludePrerelease = 
                    setIncludePrereleaseAction.IncludePrerelease
            };
        }
        
        [ReducerMethod]
        public static NuGetPackageManagerState ReduceSetMostRecentQueryResultAction(
            NuGetPackageManagerState inNuGetPackageManagerState,
            SetMostRecentQueryResultAction setMostRecentQueryResultAction)
        {
            return inNuGetPackageManagerState with
            {
                MostRecentQueryResult = 
                    setMostRecentQueryResultAction.QueryResult
            };
        }
    }
}