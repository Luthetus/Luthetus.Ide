using Fluxor;

namespace Luthetus.Ide.RazorLib.Nugets.States;

public partial record NuGetPackageManagerState
{
    private class NuGetPackageManagerStateReducer
    {
        [ReducerMethod]
        public static NuGetPackageManagerState ReduceSetSelectedProjectToModifyAction(
            NuGetPackageManagerState inState,
            SetSelectedProjectToModifyAction setSelectedProjectToModifyAction)
        {
            return inState with
            {
                SelectedProjectToModify = setSelectedProjectToModifyAction.SelectedProjectToModify
            };
        }

        [ReducerMethod]
        public static NuGetPackageManagerState ReduceSetNugetQueryAction(
            NuGetPackageManagerState inState,
            SetNugetQueryAction setNugetQueryAction)
        {
            return inState with { NugetQuery = setNugetQueryAction.NugetQuery };
        }

        [ReducerMethod]
        public static NuGetPackageManagerState ReduceSetIncludePrereleaseAction(
            NuGetPackageManagerState inState,
            SetIncludePrereleaseAction setIncludePrereleaseAction)
        {
            return inState with { IncludePrerelease = setIncludePrereleaseAction.IncludePrerelease };
        }

        [ReducerMethod]
        public static NuGetPackageManagerState ReduceSetMostRecentQueryResultAction(
            NuGetPackageManagerState inState,
            SetMostRecentQueryResultAction setMostRecentQueryResultAction)
        {
            return inState with { QueryResultList = setMostRecentQueryResultAction.QueryResultList };
        }
    }
}