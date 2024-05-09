using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FolderExplorers.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.FolderExplorers.Models;

public class LuthetusIdeFolderExplorerBackgroundTaskApi
{
    private readonly LuthetusIdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly ILuthetusIdeComponentRenderers _ideComponentRenderers;
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IDispatcher _dispatcher;

    public LuthetusIdeFolderExplorerBackgroundTaskApi(
        LuthetusIdeBackgroundTaskApi ideBackgroundTaskApi,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        ILuthetusIdeComponentRenderers ideComponentRenderers,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _fileSystemProvider = fileSystemProvider;
        _environmentProvider = environmentProvider;
        _ideComponentRenderers = ideComponentRenderers;
        _commonComponentRenderers = commonComponentRenderers;
        _treeViewService = treeViewService;
        _backgroundTaskService = backgroundTaskService;
        _dispatcher = dispatcher;
    }

    public Task SetFolderExplorerState(IAbsolutePath folderAbsolutePath)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set FolderExplorer State",
            async () => await SetFolderExplorerAsync(folderAbsolutePath));
    }

    public Task SetFolderExplorerTreeView(IAbsolutePath folderAbsolutePath)
    {
        return _backgroundTaskService.EnqueueAsync(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
            "Set FolderExplorer TreeView",
            async () => await SetFolderExplorerTreeViewAsync(folderAbsolutePath));
    }

    public async Task ShowInputFile()
    {
        await _ideBackgroundTaskApi.InputFile.RequestInputFileStateForm("Folder Explorer",
            async absolutePath =>
            {
                if (absolutePath is not null)
                    await SetFolderExplorerAsync(absolutePath);
            },
            absolutePath =>
            {
                if (absolutePath is null || !absolutePath.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            new[]
            {
                new InputFilePattern("Directory", absolutePath => absolutePath.IsDirectory)
            }.ToImmutableArray());
    }

    private async Task SetFolderExplorerAsync(IAbsolutePath folderAbsolutePath)
    {
        _dispatcher.Dispatch(new FolderExplorerState.WithAction(
            inFolderExplorerState => inFolderExplorerState with
            {
                AbsolutePath = folderAbsolutePath
            }));

        await SetFolderExplorerTreeViewAsync(folderAbsolutePath);
    }

    private async Task SetFolderExplorerTreeViewAsync(IAbsolutePath folderAbsolutePath)
    {
        _dispatcher.Dispatch(new FolderExplorerState.WithAction(inFolderExplorerState => inFolderExplorerState with
        {
            IsLoadingFolderExplorer = true
        }));

        var rootNode = new TreeViewAbsolutePath(
            folderAbsolutePath,
            _ideComponentRenderers,
            _commonComponentRenderers,
            _fileSystemProvider,
            _environmentProvider,
            true,
            true);

        await rootNode.LoadChildListAsync();

        if (!_treeViewService.TryGetTreeViewContainer(
                FolderExplorerState.TreeViewContentStateKey,
                out var treeViewState))
        {
            _treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                FolderExplorerState.TreeViewContentStateKey,
                rootNode,
                new TreeViewNoType[] { rootNode }.ToImmutableList()));
        }
        else
        {
            _treeViewService.SetRoot(FolderExplorerState.TreeViewContentStateKey, rootNode);

            _treeViewService.SetActiveNode(
                FolderExplorerState.TreeViewContentStateKey,
                rootNode,
                true,
                false);
        }

        _dispatcher.Dispatch(new FolderExplorerState.WithAction(inFolderExplorerState => inFolderExplorerState with
        {
            IsLoadingFolderExplorer = false
        }));
    }
}
