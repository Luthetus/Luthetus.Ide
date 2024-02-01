using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.CompilerServices.States;
using Luthetus.Ide.RazorLib.CompilerServices.Models;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.CompilerServices.Displays;

public partial class CompilerServiceExplorerTreeViewDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<CompilerServiceExplorerState> CompilerServiceExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorViewModelState> TextEditorViewModelStateWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorGroupState> TextEditorGroupStateWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private CompilerServiceExplorerSync CompilerServiceExplorerSync { get; set; } = null!;
    [Inject]
    private EditorSync EditorSync { get; set; } = null!;
	[Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;

    private TreeViewCommandArgs? _mostRecentTreeViewCommandArgs;
    private CompilerServiceExplorerTreeViewKeyboardEventHandler _compilerServiceExplorerTreeViewKeymap = null!;
    private CompilerServiceExplorerTreeViewMouseEventHandler _compilerServiceExplorerTreeViewMouseEventHandler = null!;

    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsStateWrap.Value.Options.IconSizeInPixels * (2.0 / 3.0));

    private static bool _hasInitialized;

    protected override void OnInitialized()
    {
        CompilerServiceExplorerStateWrap.StateChanged += RerenderAfterEventWithArgs;
        TextEditorViewModelStateWrap.StateChanged += RerenderAfterEventWithArgs;
        TextEditorGroupStateWrap.StateChanged += RerenderAfterEventWithArgs;

        _compilerServiceExplorerTreeViewKeymap = new CompilerServiceExplorerTreeViewKeyboardEventHandler(
            EditorSync,
            TreeViewService,
			BackgroundTaskService);

        _compilerServiceExplorerTreeViewMouseEventHandler = new CompilerServiceExplorerTreeViewMouseEventHandler(
            EditorSync,
            TreeViewService,
			BackgroundTaskService);

        base.OnInitialized();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            if (!_hasInitialized)
            {
                _hasInitialized = true;
                ReloadOnClick();
            }
        }

        base.OnAfterRender(firstRender);
    }

    private async void RerenderAfterEventWithArgs(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged).ConfigureAwait(false);
    }

    private async Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
    {
        _mostRecentTreeViewCommandArgs = treeViewCommandArgs;

		// The order of 'StateHasChanged(...)' and 'AddActiveDropdownKey(...)' is important.
		// The ChildContent renders nothing, unless the provider of the child content
		// re-renders now that there is a given '_mostRecentTreeViewContextMenuCommandArgs'
		await InvokeAsync(StateHasChanged).ConfigureAwait(false);

        Dispatcher.Dispatch(new DropdownState.AddActiveAction(
            CompilerServiceExplorerTreeViewContextMenu.ContextMenuEventDropdownKey));
    }

    private void ReloadOnClick()
    {
        CompilerServiceExplorerSync.SetCompilerServiceExplorerTreeView();
    }

    public void Dispose()
    {
        CompilerServiceExplorerStateWrap.StateChanged -= RerenderAfterEventWithArgs;
        TextEditorViewModelStateWrap.StateChanged -= RerenderAfterEventWithArgs;
        TextEditorGroupStateWrap.StateChanged -= RerenderAfterEventWithArgs;
    }
}