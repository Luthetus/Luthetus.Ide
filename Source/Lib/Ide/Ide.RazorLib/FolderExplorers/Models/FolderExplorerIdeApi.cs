using System.Collections.Immutable;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.FolderExplorers.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;

namespace Luthetus.Ide.RazorLib.FolderExplorers.Models;

public class FolderExplorerIdeApi
{
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IFolderExplorerService _folderExplorerService;

    public FolderExplorerIdeApi(
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        IBackgroundTaskService backgroundTaskService,
        IFolderExplorerService folderExplorerService)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;
        _ideComponentRenderers = ideComponentRenderers;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
        _backgroundTaskService = backgroundTaskService;
        _folderExplorerService = folderExplorerService;
    }

    public void SetFolderExplorerState(AbsolutePath folderAbsolutePath)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            "Set FolderExplorer State",
            async () => await SetFolderExplorerAsync(folderAbsolutePath).ConfigureAwait(false));
    }

    public void SetFolderExplorerTreeView(AbsolutePath folderAbsolutePath)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            "Set FolderExplorer TreeView",
            async () => await SetFolderExplorerTreeViewAsync(folderAbsolutePath).ConfigureAwait(false));
    }

    public void ShowInputFile()
    {
        _ideBackgroundTaskApi.InputFile.RequestInputFileStateForm(
            "Folder Explorer",
            async absolutePath =>
            {
                if (absolutePath.ExactInput is not null)
                    await SetFolderExplorerAsync(absolutePath).ConfigureAwait(false);
            },
            absolutePath =>
            {
                if (absolutePath.ExactInput is null || !absolutePath.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            new[]
            {
                new InputFilePattern("Directory", absolutePath => absolutePath.IsDirectory)
            }.ToImmutableArray());
    }

    private async Task SetFolderExplorerAsync(AbsolutePath folderAbsolutePath)
    {
        _folderExplorerService.ReduceWithAction(
            inFolderExplorerState => inFolderExplorerState with
            {
                AbsolutePath = folderAbsolutePath
            });

        await SetFolderExplorerTreeViewAsync(folderAbsolutePath).ConfigureAwait(false);
    }

    private async Task SetFolderExplorerTreeViewAsync(AbsolutePath folderAbsolutePath)
    {
        _folderExplorerService.ReduceWithAction(inFolderExplorerState => inFolderExplorerState with
        {
            IsLoadingFolderExplorer = true
        });

        var rootNode = new TreeViewAbsolutePath(
            folderAbsolutePath,
            _ideComponentRenderers,
            _commonComponentRenderers,
            _fileSystemProvider,
            _environmentProvider,
            true,
            true);

        await rootNode.LoadChildListAsync().ConfigureAwait(false);

        if (!_treeViewService.TryGetTreeViewContainer(
                FolderExplorerState.TreeViewContentStateKey,
                out var treeViewState))
        {
            _treeViewService.ReduceRegisterContainerAction(new TreeViewContainer(
                FolderExplorerState.TreeViewContentStateKey,
                rootNode,
                new() { rootNode }));
        }
        else
        {
            _treeViewService.ReduceWithRootNodeAction(FolderExplorerState.TreeViewContentStateKey, rootNode);

            _treeViewService.ReduceSetActiveNodeAction(
                FolderExplorerState.TreeViewContentStateKey,
                rootNode,
                true,
                false);
        }

        _folderExplorerService.ReduceWithAction(inFolderExplorerState => inFolderExplorerState with
        {
            IsLoadingFolderExplorer = false
        });
    }
}
