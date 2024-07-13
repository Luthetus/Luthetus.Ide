using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

[FeatureState]
public partial record TestExplorerState
{
    public static readonly Key<TreeViewContainer> TreeViewTestExplorerKey = Key<TreeViewContainer>.NewKey();

    public TestExplorerState()
    {
    }
	
	public ImmutableList<ProjectTestModel> ProjectTestModelList { get; init; } = ImmutableList<ProjectTestModel>.Empty;
}