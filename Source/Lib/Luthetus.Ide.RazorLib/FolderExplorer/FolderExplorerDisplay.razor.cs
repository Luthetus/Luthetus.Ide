using Luthetus.Common.RazorLib.Store.ApplicationOptions;
using Luthetus.Common.RazorLib.Store.DropdownCase;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Ide.ClassLib.Store.FolderExplorerCase;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.RazorLib.FolderExplorer.Classes;
using Luthetus.Ide.RazorLib.FolderExplorer.InternalComponents;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.FolderExplorer;

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
    private ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ClassLib.Menu.ICommonMenuOptionsFactory CommonMenuOptionsFactory { get; set; } = null!;

    private FolderExplorerTreeViewMouseEventHandler _folderExplorerTreeViewMouseEventHandler = null!;
    private FolderExplorerTreeViewKeyboardEventHandler _folderExplorerTreeViewKeyboardEventHandler = null!;
    private ITreeViewCommandParameter? _mostRecentTreeViewCommandParameter;

    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsStateWrap.Value.Options.IconSizeInPixels.GetValueOrDefault() *
        (2.0 / 3.0));

    protected override void OnInitialized()
    {
        FolderExplorerStateWrap.StateChanged += FolderExplorerStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged += AppOptionsStateWrapOnStateChanged;

        _folderExplorerTreeViewMouseEventHandler = new FolderExplorerTreeViewMouseEventHandler(
            Dispatcher,
            TreeViewService);

        _folderExplorerTreeViewKeyboardEventHandler = new FolderExplorerTreeViewKeyboardEventHandler(
            CommonMenuOptionsFactory,
            LuthetusIdeComponentRenderers,
            Dispatcher,
            TreeViewService);

        base.OnInitialized();
    }

    private async void FolderExplorerStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async void AppOptionsStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnTreeViewContextMenuFunc(ITreeViewCommandParameter treeViewCommandParameter)
    {
        _mostRecentTreeViewCommandParameter = treeViewCommandParameter;

        Dispatcher.Dispatch(
            new DropdownsState.AddActiveAction(
                FolderExplorerContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        FolderExplorerStateWrap.StateChanged -= FolderExplorerStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
    }
}