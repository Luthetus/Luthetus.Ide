using Fluxor;
using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.FolderExplorers.States;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.FolderExplorers.Models;
using Luthetus.Ide.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.FolderExplorers.Displays;

public partial class FolderExplorerDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<FolderExplorerState> FolderExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IMenuOptionsFactory MenuOptionsFactory { get; set; } = null!;
    [Inject]
    private EditorSync EditorSync { get; set; } = null!;
    [Inject]
    private FolderExplorerSync FolderExplorerSync { get; set; } = null!;
	[Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    private FolderExplorerTreeViewMouseEventHandler _treeViewMouseEventHandler = null!;
    private FolderExplorerTreeViewKeyboardEventHandler _treeViewKeyboardEventHandler = null!;
    private TreeViewCommandArgs? _mostRecentTreeViewCommandArgs;

    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsStateWrap.Value.Options.IconSizeInPixels * (2.0 / 3.0));

    protected override void OnInitialized()
    {
        FolderExplorerStateWrap.StateChanged += OnStateChanged;
        AppOptionsStateWrap.StateChanged += OnStateChanged;

        _treeViewMouseEventHandler = new FolderExplorerTreeViewMouseEventHandler(
            EditorSync,
            TreeViewService,
			BackgroundTaskService);

        _treeViewKeyboardEventHandler = new FolderExplorerTreeViewKeyboardEventHandler(
            EditorSync,
            MenuOptionsFactory,
            LuthetusCommonComponentRenderers,
            TreeViewService,
			BackgroundTaskService,
            EnvironmentProvider);

        base.OnInitialized();
    }

    private async void OnStateChanged(object? sender, EventArgs e) => await InvokeAsync(StateHasChanged);

    private async Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
    {
        _mostRecentTreeViewCommandArgs = treeViewCommandArgs;

        Dispatcher.Dispatch(new DropdownState.AddActiveAction(
            FolderExplorerContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        FolderExplorerStateWrap.StateChanged -= OnStateChanged;
        AppOptionsStateWrap.StateChanged -= OnStateChanged;
    }
}