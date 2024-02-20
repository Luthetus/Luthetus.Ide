using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.Nugets.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.Tests.Basis.Nugets.States;

public class NuGetPackageManagerStateActionsTests
{
    [Fact]
    public void SetSelectedProjectToModifyAction()
    {
        //public record (IDotNetProject? SelectedProjectToModify);
    }

    [Fact]
    public void SetNugetQueryAction()
    {
        //public record (string NugetQuery);
    }

    [Fact]
    public void SetIncludePrereleaseAction()
    {
        //public record (bool IncludePrerelease);
    }

    [Fact]
    public void SetMostRecentQueryResultAction()
    {
        //public record (ImmutableArray<NugetPackageRecord> QueryResultList);
    }
}