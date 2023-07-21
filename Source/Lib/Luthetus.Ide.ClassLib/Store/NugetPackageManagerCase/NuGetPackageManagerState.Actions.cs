using Luthetus.Ide.ClassLib.DotNet;
using Luthetus.Ide.ClassLib.Nuget;

namespace Luthetus.Ide.ClassLib.Store.NugetPackageManagerCase;

public partial record NuGetPackageManagerState
{
    public record SetSelectedProjectToModifyAction(IDotNetProject? SelectedProjectToModify);
    public record SetNugetQueryAction(string NugetQuery);
    public record SetIncludePrereleaseAction(bool IncludePrerelease);
    public record SetMostRecentQueryResultAction(ImmutableArray<NugetPackageRecord> QueryResult);
}