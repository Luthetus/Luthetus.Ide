using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.TreeViews.States;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Ide.RazorLib.TestExplorers.States;
using Luthetus.Ide.RazorLib.TestExplorers.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Resizes.Displays;

namespace Luthetus.Ide.RazorLib.TestExplorers.Displays;

public partial class TestExplorerDisplay : FluxorComponent
{
	[Inject]
    private IState<TestExplorerState> TestExplorerStateWrap { get; set; } = null!;
	[Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
	[Inject]
    private IState<TreeViewState> TreeViewStateWrap { get; set; } = null!;
	[Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;

	private readonly ElementDimensions _treeViewElementDimensions = new();
	private readonly ElementDimensions _detailsElementDimensions = new();

	protected override void OnInitialized()
    {
		// TODO: Supress un-used property on TreeViewStateWrap...
		// ...Its injected so that Fluxor will wire up events to re-render UI...
		// ...Preferably a different approach would be taken here.
		_ = TreeViewStateWrap;

        // TreeView ElementDimensions
		{
			var treeViewWidth = _treeViewElementDimensions.DimensionAttributeBag.Single(
	            da => da.DimensionAttributeKind == DimensionAttributeKind.Width);
	
	        treeViewWidth.DimensionUnitBag.AddRange(new[]
	        {
	            new DimensionUnit
	            {
	                Value = 50,
	                DimensionUnitKind = DimensionUnitKind.Percentage
	            },
	            new DimensionUnit
	            {
	                Value = ResizableColumn.RESIZE_HANDLE_WIDTH_IN_PIXELS / 2,
	                DimensionUnitKind = DimensionUnitKind.Pixels,
	                DimensionOperatorKind = DimensionOperatorKind.Subtract
	            }
	        });
		}

		// Details ElementDimensions
		{
			var detailsWidth = _detailsElementDimensions.DimensionAttributeBag.Single(
	            da => da.DimensionAttributeKind == DimensionAttributeKind.Width);
	
	        detailsWidth.DimensionUnitBag.AddRange(new[]
	        {
	            new DimensionUnit
	            {
	                Value = 50,
	                DimensionUnitKind = DimensionUnitKind.Percentage
	            },
	            new DimensionUnit
	            {
	                Value = ResizableColumn.RESIZE_HANDLE_WIDTH_IN_PIXELS / 2,
	                DimensionUnitKind = DimensionUnitKind.Pixels,
	                DimensionOperatorKind = DimensionOperatorKind.Subtract
	            }
	        });
		}

        base.OnInitialized();
    }
}
 
    