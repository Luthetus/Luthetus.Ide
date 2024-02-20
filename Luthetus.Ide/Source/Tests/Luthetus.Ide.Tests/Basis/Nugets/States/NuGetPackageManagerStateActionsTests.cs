using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.Nugets.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.Tests.Basis.Nugets.States;

public class NuGetPackageManagerStateActionsTests
{
    public record SetSelectedProjectToModifyAction(IDotNetProject? SelectedProjectToModify);
    public record SetNugetQueryAction(string NugetQuery);
    public record SetIncludePrereleaseAction(bool IncludePrerelease);
    public record SetMostRecentQueryResultAction(ImmutableArray<NugetPackageRecord> QueryResultList);
}