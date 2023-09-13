using Fluxor;

namespace Luthetus.Ide.RazorLib.NugetCase.NugetPackageManagerCase;

public partial record NuGetPackageManagerRegistry
{
    private class NuGetPackageManagerStateReducer
    {
        [ReducerMethod]
        public static NuGetPackageManagerRegistry ReduceSetSelectedProjectToModifyAction(
            NuGetPackageManagerRegistry inNuGetPackageManagerState,
            SetSelectedProjectToModifyAction setSelectedProjectToModifyAction)
        {
            return inNuGetPackageManagerState with
            {
                SelectedProjectToModify =
                    setSelectedProjectToModifyAction.SelectedProjectToModify
            };
        }

        [ReducerMethod]
        public static NuGetPackageManagerRegistry ReduceSetNugetQueryAction(
            NuGetPackageManagerRegistry inNuGetPackageManagerState,
            SetNugetQueryAction setNugetQueryAction)
        {
            return inNuGetPackageManagerState with
            {
                NugetQuery =
                    setNugetQueryAction.NugetQuery
            };
        }

        [ReducerMethod]
        public static NuGetPackageManagerRegistry ReduceSetIncludePrereleaseAction(
            NuGetPackageManagerRegistry inNuGetPackageManagerState,
            SetIncludePrereleaseAction setIncludePrereleaseAction)
        {
            return inNuGetPackageManagerState with
            {
                IncludePrerelease =
                    setIncludePrereleaseAction.IncludePrerelease
            };
        }

        [ReducerMethod]
        public static NuGetPackageManagerRegistry ReduceSetMostRecentQueryResultAction(
            NuGetPackageManagerRegistry inNuGetPackageManagerState,
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