using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.Store.ApplicationOptions;
using Luthetus.Common.RazorLib.Store.DropdownCase;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.Ide.ClassLib.Menu;
using Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase;
using Luthetus.TextEditor.RazorLib.Store.Group;
using Luthetus.TextEditor.RazorLib.Store.ViewModel;
using Microsoft.AspNetCore.Components;
using static Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase.CompilerServiceExplorerState;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorer;

public partial class CompilerServiceExplorerDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<CompilerServiceExplorerState> CompilerServiceExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorViewModelsCollection> TextEditorViewModelsCollectionWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorGroupsCollection> TextEditorGroupsCollectionWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsState> AppOptionsStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private CSharpCompilerService CSharpCompilerService { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers LuthetusCommonComponentRenderers { get; set; } = null!;
    [Inject]
    private IMenuOptionsFactory MenuOptionsFactory { get; set; } = null!;

    private ITreeViewCommandParameter? _mostRecentTreeViewCommandParameter;
    private CompilerServiceExplorerTreeViewKeyboardEventHandler _compilerServiceExplorerTreeViewKeymap = null!;
    private CompilerServiceExplorerTreeViewMouseEventHandler _compilerServiceExplorerTreeViewMouseEventHandler = null!;

    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsStateWrap.Value.Options.IconSizeInPixels.GetValueOrDefault() *
        (2.0 / 3.0));

    private static bool _hasInitialized;

    protected override void OnInitialized()
    {
        CompilerServiceExplorerStateWrap.StateChanged += RerenderAfterEventWithArgs;
        TextEditorViewModelsCollectionWrap.StateChanged += RerenderAfterEventWithArgs;
        TextEditorGroupsCollectionWrap.StateChanged += RerenderAfterEventWithArgs;

        CSharpCompilerService.ModelRegistered += RerenderAfterEvent;
        CSharpCompilerService.ModelDisposed += RerenderAfterEvent;
        CSharpCompilerService.ModelParsed += RerenderAfterEvent;

        _compilerServiceExplorerTreeViewKeymap = new CompilerServiceExplorerTreeViewKeyboardEventHandler(
            MenuOptionsFactory,
            LuthetusCommonComponentRenderers,
            Dispatcher,
            TreeViewService);

        _compilerServiceExplorerTreeViewMouseEventHandler =
            new CompilerServiceExplorerTreeViewMouseEventHandler(
                Dispatcher,
                TreeViewService);

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
        await InvokeAsync(StateHasChanged);
    }

    private async void RerenderAfterEvent()
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnTreeViewContextMenuFunc(ITreeViewCommandParameter treeViewCommandParameter)
    {
        _mostRecentTreeViewCommandParameter = treeViewCommandParameter;

        Dispatcher.Dispatch(new DropdownsState.AddActiveAction(
            CompilerServiceExplorerContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }

    private void ReloadOnClick()
    {
        Dispatcher.Dispatch(new SetCompilerServiceExplorerAction());
    }

    public void Dispose()
    {
        CompilerServiceExplorerStateWrap.StateChanged -= RerenderAfterEventWithArgs;
        TextEditorViewModelsCollectionWrap.StateChanged -= RerenderAfterEventWithArgs;
        TextEditorGroupsCollectionWrap.StateChanged -= RerenderAfterEventWithArgs;
        
        CSharpCompilerService.ModelRegistered -= RerenderAfterEvent;
        CSharpCompilerService.ModelDisposed -= RerenderAfterEvent;
        CSharpCompilerService.ModelParsed -= RerenderAfterEvent;
    }
}