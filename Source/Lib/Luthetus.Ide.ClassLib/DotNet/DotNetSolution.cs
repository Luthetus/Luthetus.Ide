using Luthetus.Ide.ClassLib.Namespaces;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.DotNet;

public record DotNetSolution(
    NamespacePath NamespacePath,
    ImmutableList<IDotNetProject> DotNetProjects,
    ImmutableList<DotNetSolutionFolder> SolutionFolders,
    DotNetSolutionGlobalSection DotNetSolutionGlobalSection)
{
    public Guid Id { get; init; } = Guid.NewGuid();
}