using Luthetus.CompilerServices.Lang.DotNetSolution;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.NugetCase.NugetPackageManagerCase;

public partial record NuGetPackageManagerRegistry
{
    public record SetSelectedProjectToModifyAction(IDotNetProject? SelectedProjectToModify);
    public record SetNugetQueryAction(string NugetQuery);
    public record SetIncludePrereleaseAction(bool IncludePrerelease);
    public record SetMostRecentQueryResultAction(ImmutableArray<NugetPackageRecord> QueryResult);
}