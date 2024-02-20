using Fluxor;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;

namespace Luthetus.Ide.Tests.Basis.TestExplorers.States;

[FeatureState]
public partial record TestExplorerStateMainTests
{
    public static readonly Key<TreeViewContainer> TreeViewTestExplorerKey = Key<TreeViewContainer>.NewKey();

    public TestExplorerState()
    {
    }
	
	public ImmutableList<ProjectTestModel> ProjectTestModelList { get; init; } = ImmutableList<ProjectTestModel>.Empty;
}