using System.Collections.Immutable;
using BlazorCommon.RazorLib.Dialog;
using BlazorCommon.RazorLib.Dimensions;
using BlazorCommon.RazorLib.Store.DropdownCase;
using BlazorCommon.RazorLib.TreeView;
using BlazorCommon.RazorLib.TreeView.Commands;
using BlazorCommon.RazorLib.TreeView.TreeViewClasses;
using BlazorStudio.ClassLib.Store.InputFileCase;
using BlazorStudio.ClassLib.TreeViewImplementations;
using Fluxor;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.RazorLib.InputFile.Classes;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.InputFile.InternalComponents;

public partial class InputFileSidebar : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ILuthetusIdeComponentRenderers BlazorStudioComponentRenderers { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    [CascadingParameter(Name = "SetInputFileContentTreeViewRootFunc")]
    public Func<IAbsoluteFilePath, Task> SetInputFileContentTreeViewRootFunc { get; set; } = null!;
    [CascadingParameter]
    public InputFileTreeViewMouseEventHandler InputFileTreeViewMouseEventHandler { get; set; } = null!;
    [CascadingParameter]
    public InputFileTreeViewKeyboardEventHandler InputFileTreeViewKeyboardEventHandler { get; set; } = null!;
    [CascadingParameter]
    public InputFileState InputFileState { get; set; } = null!;
    [CascadingParameter]
    public DialogRecord DialogRecord { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsoluteFilePath?> SetSelectedAbsoluteFilePath { get; set; } = null!;

    public static readonly TreeViewStateKey TreeViewInputFileSidebarStateKey =
        TreeViewStateKey.NewTreeViewStateKey();

    private ITreeViewCommandParameter? _mostRecentTreeViewCommandParameter;

    protected override void OnInitialized()
    {
        var directoryHomeNode = new TreeViewAbsoluteFilePath(
            EnvironmentProvider.HomeDirectoryAbsoluteFilePath,
            BlazorStudioComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            true,
            false);

        var directoryRootNode = new TreeViewAbsoluteFilePath(
            EnvironmentProvider.RootDirectoryAbsoluteFilePath,
            BlazorStudioComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            true,
            false);

        var adhocRootNode = TreeViewAdhoc.ConstructTreeViewAdhoc(
            directoryHomeNode,
            directoryRootNode);

        if (!TreeViewService.TryGetTreeViewState(
                TreeViewInputFileSidebarStateKey, out var treeViewState))
        {
            TreeViewService.RegisterTreeViewState(new TreeViewState(
                TreeViewInputFileSidebarStateKey,
                adhocRootNode,
                directoryHomeNode,
                ImmutableList<TreeViewNoType>.Empty));
        }

        base.OnInitialized();
    }

    private async Task OnTreeViewContextMenuFunc(ITreeViewCommandParameter treeViewCommandParameter)
    {
        _mostRecentTreeViewCommandParameter = treeViewCommandParameter;

        Dispatcher.Dispatch(
            new DropdownsState.AddActiveAction(
                InputFileContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }
}