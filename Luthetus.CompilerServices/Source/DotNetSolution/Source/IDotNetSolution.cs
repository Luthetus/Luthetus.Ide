using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete.DotNetSolutionGlobalSectionTypes;
using Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete.RewriteForImmutability;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution;

public interface IDotNetSolution
{
    public Key<DotNetSolutionModel> DotNetSolutionModelKey { get; }
    public NamespacePath NamespacePath { get; }
    public string SolutionFileContents { get; }
    public ImmutableArray<IDotNetProject> DotNetProjects { get; }
    public ImmutableArray<DotNetSolutionFolder> SolutionFolders { get; }
    public ImmutableArray<NestedProjectEntry> NestedProjectEntries { get; }
}