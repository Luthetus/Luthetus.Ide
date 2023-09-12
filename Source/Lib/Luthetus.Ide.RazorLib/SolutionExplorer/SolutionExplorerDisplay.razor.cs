using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.Dialog;
using Luthetus.Common.RazorLib.Store.AppOptionsCase;
using Luthetus.Common.RazorLib.Store.DialogCase;
using Luthetus.Common.RazorLib.Store.DropdownCase;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Ide.ClassLib.Menu;
using Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;
using Luthetus.Ide.RazorLib.DotNetSolutionForm;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.SolutionExplorer;

public partial class SolutionExplorerDisplay : FluxorComponent
{
    [Inject]
    private IState<AppOptionsRegistry> AppOptionsRegistryWrap { get; set; } = null!;
    [Inject]
    private IState<DotNetSolutionRegistry> DotNetSolutionStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IMenuOptionsFactory MenuOptionsFactory { get; set; } = null!;

    private ITreeViewCommandParameter? _mostRecentTreeViewCommandParameter;
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
            MenuOptionsFactory,
            LuthetusCommonComponentRenderers,
            Dispatcher,
            TreeViewService);

        _solutionExplorerTreeViewMouseEventHandler =
            new SolutionExplorerTreeViewMouseEventHandler(
                Dispatcher,
                TreeViewService);

        base.OnInitialized();
    }

    private async void DotNetSolutionStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnTreeViewContextMenuFunc(ITreeViewCommandParameter treeViewCommandParameter)
    {
        _mostRecentTreeViewCommandParameter = treeViewCommandParameter;

        Dispatcher.Dispatch(new DropdownRegistry.AddActiveAction(
            SolutionExplorerContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }

    private void OpenNewDotNetSolutionDialog()
    {
        var dialogRecord = new DialogRecord(
            DialogKey.NewKey(),
            "New .NET Solution",
            typeof(DotNetSolutionFormDisplay),
            null,
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