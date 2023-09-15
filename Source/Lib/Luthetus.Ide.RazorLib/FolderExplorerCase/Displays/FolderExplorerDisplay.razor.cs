using Fluxor;
using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;
using Luthetus.Ide.RazorLib.FolderExplorerCase.Models;
using Luthetus.Ide.RazorLib.MenuCase.Models;
using Luthetus.Ide.RazorLib.FolderExplorerCase.States;
using Luthetus.Ide.RazorLib.EditorCase.States;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.Dropdown.States;

namespace Luthetus.Ide.RazorLib.FolderExplorerCase.Displays;

public partial class FolderExplorerDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<FolderExplorerState> FolderExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsRegistry> AppOptionsRegistryWrap { get; set; } = null!;
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
    [Inject]
    private EditorSync EditorSync { get; set; } = null!;
    [Inject]
    private FolderExplorerSync FolderExplorerSync { get; set; } = null!;

    private FolderExplorerTreeViewMouseEventHandler _folderExplorerTreeViewMouseEventHandler = null!;
    private FolderExplorerTreeViewKeyboardEventHandler _folderExplorerTreeViewKeyboardEventHandler = null!;
    private ITreeViewCommandParameter? _mostRecentTreeViewCommandParameter;

    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsRegistryWrap.Value.Options.IconSizeInPixels.GetValueOrDefault() *
        (2.0 / 3.0));

    protected override void OnInitialized()
    {
        FolderExplorerStateWrap.StateChanged += FolderExplorerStateWrapOnStateChanged;
        AppOptionsRegistryWrap.StateChanged += AppOptionsStateWrapOnStateChanged;

        _folderExplorerTreeViewMouseEventHandler = new FolderExplorerTreeViewMouseEventHandler(
            EditorSync,
            Dispatcher,
            TreeViewService);

        _folderExplorerTreeViewKeyboardEventHandler = new FolderExplorerTreeViewKeyboardEventHandler(
            EditorSync,
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
        AppOptionsRegistryWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
    }
}