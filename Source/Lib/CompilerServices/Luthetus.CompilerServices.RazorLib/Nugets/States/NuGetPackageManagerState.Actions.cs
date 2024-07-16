using System.Collections.Immutable;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.Nugets.Models;

namespace Luthetus.CompilerServices.RazorLib.Nugets.States;

public partial record NuGetPackageManagerState
{
    public record SetSelectedProjectToModifyAction(IDotNetProject? SelectedProjectToModify);
    public record SetNugetQueryAction(string NugetQuery);
    public record SetIncludePrereleaseAction(bool IncludePrerelease);
    public record SetMostRecentQueryResultAction(ImmutableArray<NugetPackageRecord> QueryResultList);
}