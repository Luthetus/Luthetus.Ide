using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.Store.AppOptionsCase;
using Luthetus.Common.RazorLib.Store.DropdownCase;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.Menu;
using Luthetus.Ide.ClassLib.Store.FolderExplorerCase;
using Luthetus.Ide.RazorLib.FolderExplorer.Classes;
using Luthetus.Ide.RazorLib.FolderExplorer.InternalComponents;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.FolderExplorer;

public partial class FolderExplorerDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<FolderExplorerRegistry> FolderExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsRegistry> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IMenuOptionsFactory MenuOptionsFactory { get; set; } = null!;

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
            MenuOptionsFactory,
            LuthetusCommonComponentRenderers,
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

        Dispatcher.Dispatch(new DropdownRegistry.AddActiveAction(
            FolderExplorerContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        FolderExplorerStateWrap.StateChanged -= FolderExplorerStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
    }
}