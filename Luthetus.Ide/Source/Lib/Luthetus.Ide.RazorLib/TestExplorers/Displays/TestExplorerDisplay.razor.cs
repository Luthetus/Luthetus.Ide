using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.TreeViews.States;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
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

	private ElementDimensions _treeViewElementDimensions = new();
	private ElementDimensions _detailsElementDimensions = new();
	private TestExplorerRenderBatch _testExplorerRenderBatch;

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
	                Value = 60,
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
	                Value = 40,
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
 
    