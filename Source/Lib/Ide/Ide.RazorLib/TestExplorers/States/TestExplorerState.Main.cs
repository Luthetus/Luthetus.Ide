using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

[FeatureState]
public partial record TestExplorerState(
	string? SolutionFilePath,
	int TotalTestCount,
	int NotRanTestCount,
	int PassedTestCount,
	int FailedTestCount)
{
    public static readonly Key<TreeViewContainer> TreeViewTestExplorerKey = Key<TreeViewContainer>.NewKey();

    public TestExplorerState() : this(
    	SolutionFilePath: null,
    	TotalTestCount: 0,
    	NotRanTestCount: 0,
    	PassedTestCount: 0,
    	FailedTestCount: 0)
    {
    }
	
	public ImmutableList<ProjectTestModel> ProjectTestModelList { get; init; } = ImmutableList<ProjectTestModel>.Empty;
}