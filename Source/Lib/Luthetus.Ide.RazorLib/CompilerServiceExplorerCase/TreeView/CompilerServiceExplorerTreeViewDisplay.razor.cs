using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.Store.AppOptionsCase;
using Luthetus.Common.RazorLib.Store.DropdownCase;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase;
using Luthetus.Ide.RazorLib.MenuCase;
using Luthetus.TextEditor.RazorLib.Store.Group;
using Luthetus.TextEditor.RazorLib.Store.ViewModel;
using Microsoft.AspNetCore.Components;
using static Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.CompilerServiceExplorerRegistry;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.TreeView;

public partial class CompilerServiceExplorerTreeViewDisplay : ComponentBase, IDisposable
{
    [Inject]
    private IState<CompilerServiceExplorerRegistry> CompilerServiceExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorViewModelRegistry> TextEditorViewModelRegistryWrap { get; set; } = null!;
    [Inject]
    private IState<TextEditorGroupRegistry> TextEditorGroupRegistryWrap { get; set; } = null!;
    [Inject]
    private IState<AppOptionsRegistry> AppOptionsRegistryWrap { get; set; } = null!;
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
        AppOptionsRegistryWrap.Value.Options.IconSizeInPixels.GetValueOrDefault() *
        (2.0 / 3.0));

    private static bool _hasInitialized;

    protected override void OnInitialized()
    {
        CompilerServiceExplorerStateWrap.StateChanged += RerenderAfterEventWithArgs;
        TextEditorViewModelRegistryWrap.StateChanged += RerenderAfterEventWithArgs;
        TextEditorGroupRegistryWrap.StateChanged += RerenderAfterEventWithArgs;

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

        Dispatcher.Dispatch(new DropdownRegistry.AddActiveAction(
            CompilerServiceExplorerTreeViewContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }

    private void ReloadOnClick()
    {
        Dispatcher.Dispatch(new SetCompilerServiceExplorerAction());
    }

    public void Dispose()
    {
        CompilerServiceExplorerStateWrap.StateChanged -= RerenderAfterEventWithArgs;
        TextEditorViewModelRegistryWrap.StateChanged -= RerenderAfterEventWithArgs;
        TextEditorGroupRegistryWrap.StateChanged -= RerenderAfterEventWithArgs;

        CSharpCompilerService.ModelRegistered -= RerenderAfterEvent;
        CSharpCompilerService.ModelDisposed -= RerenderAfterEvent;
        CSharpCompilerService.ModelParsed -= RerenderAfterEvent;
    }
}