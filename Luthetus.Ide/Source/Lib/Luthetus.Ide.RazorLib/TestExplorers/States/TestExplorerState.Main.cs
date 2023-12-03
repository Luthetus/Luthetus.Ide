using Fluxor;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

[FeatureState]
public partial record TestExplorerState(
    Key<DotNetSolutionModel>? DotNetSolutionModelKey,
    int IsExecutingAsyncTaskLinks)
{
    public static readonly Key<TreeViewContainer> TreeViewTestExplorerKey = Key<TreeViewContainer>.NewKey();

    public TestExplorerState()
    {
    }
	
	private ImmutableList<ProjectTestModel> ProjectTestModelBag { get; init; } = ImmutableList<ProjectTestModel>.Empty;

    public DotNetSolutionModel? DotNetSolutionModel => DotNetSolutionsBag.FirstOrDefault(x =>
        x.Key == DotNetSolutionModelKey);
}