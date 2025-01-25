using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Extensions.DotNet.TestExplorers.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.States;

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
    	// TreeView ElementDimensions
		{
			var treeViewWidth = TreeViewElementDimensions.DimensionAttributeList.Single(
				da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

			treeViewWidth.DimensionUnitList.AddRange(new[]
			{
				new DimensionUnit(
					50,
					DimensionUnitKind.Percentage),
                new DimensionUnit(
                	0,
                	DimensionUnitKind.Pixels,
                	DimensionOperatorKind.Subtract,
                	DimensionUnitFacts.Purposes.OFFSET),
			});
		}

		// Details ElementDimensions
		{
			var detailsWidth = DetailsElementDimensions.DimensionAttributeList.Single(
				da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

			detailsWidth.DimensionUnitList.AddRange(new[]
			{
				new DimensionUnit(
					50,
					DimensionUnitKind.Percentage),
                new DimensionUnit(
                	0,
                	DimensionUnitKind.Pixels,
                	DimensionOperatorKind.Subtract,
                	DimensionUnitFacts.Purposes.OFFSET),
			});
		}
    }
	
	public ImmutableList<ProjectTestModel> ProjectTestModelList { get; init; } = ImmutableList<ProjectTestModel>.Empty;
	
	public ElementDimensions TreeViewElementDimensions { get; init; } = new();
	public ElementDimensions DetailsElementDimensions { get; init; } = new();
}