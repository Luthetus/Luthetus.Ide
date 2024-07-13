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
	ImmutableHashSet<string> NotRanTestHashSet,
	ImmutableHashSet<string> PassedTestHashSet,
	ImmutableHashSet<string> FailedTestHashSet)
{
    public static readonly Key<TreeViewContainer> TreeViewTestExplorerKey = Key<TreeViewContainer>.NewKey();

    public TestExplorerState() : this(
    	SolutionFilePath: null,
    	TotalTestCount: 0,
    	NotRanTestHashSet: ImmutableHashSet<string>.Empty,
    	PassedTestHashSet: ImmutableHashSet<string>.Empty,
    	FailedTestHashSet: ImmutableHashSet<string>.Empty)
    {
    }
	
	public ImmutableList<ProjectTestModel> ProjectTestModelList { get; init; } = ImmutableList<ProjectTestModel>.Empty;
}