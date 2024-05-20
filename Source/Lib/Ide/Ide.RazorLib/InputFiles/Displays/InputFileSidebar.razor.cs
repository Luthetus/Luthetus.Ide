using Fluxor;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.InputFiles.Displays;

public partial class InputFileSidebar : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ILuthetusIdeComponentRenderers IdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    [CascadingParameter(Name="SetInputFileContentTreeViewRootFunc")]
    public Func<IAbsolutePath, Task> SetInputFileContentTreeViewRootFunc { get; set; } = null!;
    [CascadingParameter]
    public InputFileTreeViewMouseEventHandler InputFileTreeViewMouseEventHandler { get; set; } = null!;
    [CascadingParameter]
    public InputFileTreeViewKeyboardEventHandler InputFileTreeViewKeyboardEventHandler { get; set; } = null!;
    [CascadingParameter]
    public InputFileState InputFileState { get; set; } = null!;
    [CascadingParameter]
    public IDialog DialogRecord { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions ElementDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Action<IAbsolutePath?> SetSelectedAbsolutePath { get; set; } = null!;

    public static readonly Key<TreeViewContainer> TreeViewContainerKey = Key<TreeViewContainer>.NewKey();

    private TreeViewCommandArgs? _mostRecentTreeViewCommandArgs;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var directoryHomeNode = new TreeViewAbsolutePath(
                EnvironmentProvider.HomeDirectoryAbsolutePath,
                IdeComponentRenderers,
                CommonComponentRenderers,
                FileSystemProvider,
                EnvironmentProvider,
                true,
                false);

            var directoryRootNode = new TreeViewAbsolutePath(
                EnvironmentProvider.RootDirectoryAbsolutePath,
                IdeComponentRenderers,
                CommonComponentRenderers,
                FileSystemProvider,
                EnvironmentProvider,
                true,
                false);

            var adhocRootNode = TreeViewAdhoc.ConstructTreeViewAdhoc(directoryHomeNode, directoryRootNode);

            if (!TreeViewService.TryGetTreeViewContainer(TreeViewContainerKey, out var treeViewContainer))
            {
                TreeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                    TreeViewContainerKey,
                    adhocRootNode,
                    directoryHomeNode is null
                        ? ImmutableList<TreeViewNoType>.Empty
                        : new TreeViewNoType[] { directoryHomeNode }.ToImmutableList()));
            }
        }

        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnTreeViewContextMenuFunc(TreeViewCommandArgs treeViewCommandArgs)
    {
        _mostRecentTreeViewCommandArgs = treeViewCommandArgs;

		// The order of 'StateHasChanged(...)' and 'AddActiveDropdownKey(...)' is important.
		// The ChildContent renders nothing, unless the provider of the child content
		// re-renders now that there is a given '_mostRecentTreeViewContextMenuCommandArgs'
		await InvokeAsync(StateHasChanged);

        Dispatcher.Dispatch(new DropdownState.AddActiveAction(InputFileContextMenu.ContextMenuKey));
    }
}