using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;

namespace Luthetus.CompilerServices.DotNetSolution.Models;

public record DotNetSolutionModel : IDotNetSolution
{
    public DotNetSolutionModel(
        AbsolutePath absolutePath,
        DotNetSolutionHeader dotNetSolutionHeader,
        ImmutableArray<IDotNetProject> dotNetProjectList,
        ImmutableArray<SolutionFolder> solutionFolderList,
        ImmutableArray<NestedProjectEntry> nestedProjectEntryList,
        DotNetSolutionGlobal dotNetSolutionGlobal,
        string solutionFileContents)
    {
        AbsolutePath = absolutePath;
        DotNetSolutionHeader = dotNetSolutionHeader;
        DotNetProjectList = dotNetProjectList;
        SolutionFolderList = solutionFolderList;
        NestedProjectEntryList = nestedProjectEntryList;
        DotNetSolutionGlobal = dotNetSolutionGlobal;
        SolutionFileContents = solutionFileContents;
    }

    public Key<DotNetSolutionModel> Key { get; init; }
    public AbsolutePath AbsolutePath { get; init; }
    public DotNetSolutionHeader DotNetSolutionHeader { get; init; }
    public ImmutableArray<IDotNetProject> DotNetProjectList { get; init; }
    public ImmutableArray<SolutionFolder> SolutionFolderList { get; init; }
    public ImmutableArray<NestedProjectEntry> NestedProjectEntryList { get; init; }
    public DotNetSolutionGlobal DotNetSolutionGlobal { get; init; }
    public string SolutionFileContents { get; init; }

    public NamespacePath NamespacePath => new(string.Empty, AbsolutePath);
}
