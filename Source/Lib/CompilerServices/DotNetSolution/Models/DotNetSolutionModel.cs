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
        List<IDotNetProject> dotNetProjectList,
		List<SolutionFolder> solutionFolderList,
		List<NestedProjectEntry> nestedProjectEntryList,
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
    public List<IDotNetProject> DotNetProjectList { get; set; }
    public List<SolutionFolder> SolutionFolderList { get; init; }
    public List<NestedProjectEntry> NestedProjectEntryList { get; init; }
    public DotNetSolutionGlobal DotNetSolutionGlobal { get; init; }
    public string SolutionFileContents { get; init; }

    public NamespacePath NamespacePath => new(string.Empty, AbsolutePath);
}
