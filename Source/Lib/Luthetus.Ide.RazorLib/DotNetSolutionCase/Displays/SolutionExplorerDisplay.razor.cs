using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialog.Models;
using Luthetus.Common.RazorLib.Dialog.States;
using Luthetus.Common.RazorLib.Dropdown.States;
using Luthetus.Common.RazorLib.KeyCase;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.Models;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.States;
using Luthetus.Ide.RazorLib.DotNetSolutionCase.Scenes;
using Luthetus.Ide.RazorLib.EditorCase.States;
using Luthetus.Ide.RazorLib.MenuCase.Models;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Commands.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.Displays;

public partial class SolutionExplorerDisplay : FluxorComponent
{
    [Inject]
    private IState<AppOptionsRegistry> AppOptionsRegistryWrap { get; set; } = null!;
    [Inject]
    private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IMenuOptionsFactory MenuOptionsFactory { get; set; } = null!;
    [Inject]
    private DotNetSolutionSync DotNetSolutionSync { get; set; } = null!;
    [Inject]
    private EditorSync EditorSync { get; set; } = null!;

    private TreeViewCommandParameter? _mostRecentTreeViewCommandParameter;
    private SolutionExplorerTreeViewKeyboardEventHandler _solutionExplorerTreeViewKeymap = null!;
    private SolutionExplorerTreeViewMouseEventHandler _solutionExplorerTreeViewMouseEventHandler = null!;
    private bool _disposed;

    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsRegistryWrap.Value.Options.IconSizeInPixels.GetValueOrDefault() *
        (2.0 / 3.0));

    protected override void OnInitialized()
    {
        DotNetSolutionStateWrap.StateChanged += DotNetSolutionStateWrapOnStateChanged;

        _solutionExplorerTreeViewKeymap = new SolutionExplorerTreeViewKeyboardEventHandler(
            EditorSync,
            MenuOptionsFactory,
            LuthetusCommonComponentRenderers,
            Dispatcher,
            TreeViewService);

        _solutionExplorerTreeViewMouseEventHandler =
            new SolutionExplorerTreeViewMouseEventHandler(
                EditorSync,
                Dispatcher,
                TreeViewService);

        base.OnInitialized();
    }

    private async void DotNetSolutionStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnTreeViewContextMenuFunc(TreeViewCommandParameter treeViewCommandParameter)
    {
        _mostRecentTreeViewCommandParameter = treeViewCommandParameter;

        Dispatcher.Dispatch(new DropdownRegistry.AddActiveAction(
            SolutionExplorerContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }

    private void OpenNewDotNetSolutionDialog()
    {
        var dialogRecord = new DialogRecord(
            Key<DialogRecord>.NewKey(),
            "New .NET Solution",
            typeof(DotNetSolutionFormDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(DotNetSolutionFormDisplay.Viewable),
                    new DotNetSolutionFormScene()
                }
            },
            null)
        {
            IsResizable = true
        };

        Dispatcher.Dispatch(new DialogRegistry.RegisterAction(
            dialogRecord));
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _disposed = true;

            DotNetSolutionStateWrap.StateChanged -= DotNetSolutionStateWrapOnStateChanged;
        }

        base.Dispose(disposing);
    }
}