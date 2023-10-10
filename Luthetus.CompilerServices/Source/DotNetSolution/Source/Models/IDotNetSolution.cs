using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Models;

public interface IDotNetSolution
{
    public Key<DotNetSolutionModel> Key { get; init; }
    public IAbsolutePath AbsolutePath { get; init; }
    public DotNetSolutionHeader DotNetSolutionHeader { get; init; }
    public ImmutableArray<IDotNetProject> DotNetProjectBag { get; }
    public ImmutableArray<SolutionFolder> SolutionFolderBag { get; init; }
    public ImmutableArray<NestedProjectEntry> NestedProjectEntryBag { get; init; }
    public DotNetSolutionGlobal DotNetSolutionGlobal { get; init; }
    public string SolutionFileContents { get; }

    public NamespacePath NamespacePath => new(string.Empty, AbsolutePath);

    
}
