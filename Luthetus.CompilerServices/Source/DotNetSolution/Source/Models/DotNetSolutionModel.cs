using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Models;

public record DotNetSolutionModel : IDotNetSolution
{
    public DotNetSolutionModel(
        IAbsolutePath absolutePath,
        DotNetSolutionHeader dotNetSolutionHeader,
        ImmutableArray<IDotNetProject> dotNetProjectBag,
        ImmutableArray<SolutionFolder> solutionFolderBag,
        ImmutableArray<NestedProjectEntry> nestedProjectEntryBag,
        DotNetSolutionGlobal dotNetSolutionGlobal,
        string solutionFileContents)
    {
        AbsolutePath = absolutePath;
        DotNetSolutionHeader = dotNetSolutionHeader;
        DotNetProjectBag = dotNetProjectBag;
        SolutionFolderBag = solutionFolderBag;
        NestedProjectEntryBag = nestedProjectEntryBag;
        DotNetSolutionGlobal = dotNetSolutionGlobal;
        SolutionFileContents = solutionFileContents;
    }

    public Key<DotNetSolutionModel> Key { get; init; }
    public IAbsolutePath AbsolutePath { get; init; }
    public DotNetSolutionHeader DotNetSolutionHeader { get; init; }
    public ImmutableArray<IDotNetProject> DotNetProjectBag { get; init; }
    public ImmutableArray<SolutionFolder> SolutionFolderBag { get; init; }
    public ImmutableArray<NestedProjectEntry> NestedProjectEntryBag { get; init; }
    public DotNetSolutionGlobal DotNetSolutionGlobal { get; init; }
    public string SolutionFileContents { get; init; }

    public NamespacePath NamespacePath => new(string.Empty, AbsolutePath);

}
