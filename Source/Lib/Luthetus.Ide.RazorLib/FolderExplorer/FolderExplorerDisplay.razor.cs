using System.Collections.Immutable;
using Luthetus.Common.RazorLib.BackgroundTaskCase;
using Luthetus.Common.RazorLib.Store.AccountCase;
using Luthetus.Common.RazorLib.Store.ApplicationOptions;
using Luthetus.Common.RazorLib.Store.DropdownCase;
using Luthetus.Common.RazorLib.TreeView;
using Luthetus.Common.RazorLib.TreeView.Commands;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.Store.FolderExplorerCase;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Store.TerminalCase;
using Luthetus.Ide.RazorLib.FolderExplorer.Classes;
using Luthetus.Ide.RazorLib.FolderExplorer.InternalComponents;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

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
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ClassLib.Menu.ICommonMenuOptionsFactory CommonMenuOptionsFactory { get; set; } = null!;

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    private const string FOLDER_EXPLORER_ABSOLUTE_PATH_STRING = @"C:\Users\hunte\Repos\CHelloWorld";

    public static readonly TreeViewStateKey TreeViewFolderExplorerContentStateKey =
        TreeViewStateKey.NewTreeViewStateKey();

    private FolderExplorerTreeViewMouseEventHandler _folderExplorerTreeViewMouseEventHandler = null!;
    private FolderExplorerTreeViewKeyboardEventHandler _folderExplorerTreeViewKeyboardEventHandler = null!;
    private ITreeViewCommandParameter? _mostRecentTreeViewCommandParameter;

    private TreeViewNodeKey _previousRootTreeViewNodeKey = TreeViewNodeKey.Empty;

    private int OffsetPerDepthInPixels => (int)Math.Ceiling(
        AppOptionsStateWrap.Value.Options.IconSizeInPixels.GetValueOrDefault() *
        (2.0 / 3.0));

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationStateTask is not null)
        {
            var authenticationState = await AuthenticationStateTask;

            var subClaim = authenticationState.User.FindFirst(
                AccountState.SUB_CLAIM_NAME);

            if (subClaim is null)
                return;

            Dispatcher.Dispatch(new AccountState.AccountStateWithAction(
                inAccountState => inAccountState with
                {
                    GroupName = subClaim.Value
                }));

            var folderExplorerState = FolderExplorerStateWrap.Value;

            var treeViewStateFound = TreeViewService.TryGetTreeViewState(
                TreeViewFolderExplorerContentStateKey,
                out var treeViewState);

            if (treeViewStateFound &&
                treeViewState is not null &&
                _previousRootTreeViewNodeKey != treeViewState.RootNode.TreeViewNodeKey)
            {
                FolderExplorerStateWrapOnStateChanged(null, EventArgs.Empty);
            }
            else
            {
                if (folderExplorerState.AbsoluteFilePath is not null)
                    await SetFolderExplorerTreeViewRootAsync(folderExplorerState.AbsoluteFilePath);
            }
        }

        await base.OnInitializedAsync();
    }

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
        var folderExplorerState = FolderExplorerStateWrap.Value;

        if (folderExplorerState.AbsoluteFilePath is not null)
            await SetFolderExplorerTreeViewRootAsync(folderExplorerState.AbsoluteFilePath);

        await InvokeAsync(StateHasChanged);
    }

    private async void AppOptionsStateWrapOnStateChanged(object? sender, EventArgs e)
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task SetFolderExplorerTreeViewRootAsync(IAbsoluteFilePath absoluteFilePath)
    {
        var rootNode = new TreeViewAbsoluteFilePath(
            absoluteFilePath,
            LuthetusIdeComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            true,
            true);

        _previousRootTreeViewNodeKey = rootNode.TreeViewNodeKey;

        await rootNode.LoadChildrenAsync();

        if (!TreeViewService.TryGetTreeViewState(
                TreeViewFolderExplorerContentStateKey,
                out var treeViewState))
        {
            TreeViewService.RegisterTreeViewState(new TreeViewState(
                TreeViewFolderExplorerContentStateKey,
                rootNode,
                rootNode,
                ImmutableList<TreeViewNoType>.Empty));
        }
        else
        {
            TreeViewService.SetRoot(
                TreeViewFolderExplorerContentStateKey,
                rootNode);

            TreeViewService.SetActiveNode(
                TreeViewFolderExplorerContentStateKey,
                rootNode);
        }
    }

    private async Task OnTreeViewContextMenuFunc(ITreeViewCommandParameter treeViewCommandParameter)
    {
        _mostRecentTreeViewCommandParameter = treeViewCommandParameter;

        Dispatcher.Dispatch(
            new DropdownsState.AddActiveAction(
                FolderExplorerContextMenu.ContextMenuEventDropdownKey));

        await InvokeAsync(StateHasChanged);
    }

    private async Task SetFolderExplorerOnClickAsync()
    {
        var absoluteFilePath = new AbsoluteFilePath(
            FOLDER_EXPLORER_ABSOLUTE_PATH_STRING,
            true,
            EnvironmentProvider);

        Dispatcher.Dispatch(
            new SetFolderExplorerStateAction(absoluteFilePath));
    }

    public void Dispose()
    {
        FolderExplorerStateWrap.StateChanged -= FolderExplorerStateWrapOnStateChanged;
        AppOptionsStateWrap.StateChanged -= AppOptionsStateWrapOnStateChanged;
    }
}