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
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.TestExplorers.States;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Resizes.Displays;

namespace Luthetus.Ide.RazorLib.TestExplorers.Displays;

public partial class TestExplorerTreeViewDisplay : ComponentBase
{
	[Inject]
    private IState<TestExplorerState> TestExplorerStateWrap { get; set; } = null!;
	[Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
	[Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
	[Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	
	[Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;

    private TreeViewCommandArgs? _mostRecentTreeViewCommandArgs;
	private TreeViewKeyboardEventHandler _treeViewKeyboardEventHandler = null!;
    private TreeViewMouseEventHandler _treeViewMouseEventHandler = null!;
	private ElementDimensions _treeViewElementDimensions = new();
	private ElementDimensions _detailsElementDimensions = new();

	private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsStateWrap.Value.Options.IconSizeInPixels * (2.0 / 3.0));

	protected override void OnInitialized()
    {
        _treeViewKeyboardEventHandler = new TreeViewKeyboardEventHandler(
            TreeViewService,
			BackgroundTaskService);

        _treeViewMouseEventHandler = new TreeViewMouseEventHandler(
            TreeViewService,
			BackgroundTaskService);

        base.OnInitialized();
    }

	private async Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
    {
        _mostRecentTreeViewCommandArgs = treeViewCommandArgs;

        Dispatcher.Dispatch(new DropdownState.AddActiveAction(
            TestExplorerContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }
}