using System.Collections.Concurrent;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;

namespace Luthetus.Ide.RazorLib.FolderExplorers.Models;

public class FolderExplorerIdeApi : IBackgroundTaskGroup
{
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IEnvironmentProvider _environmentProvider;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly BackgroundTaskService _backgroundTaskService;
    private readonly IFolderExplorerService _folderExplorerService;

    public FolderExplorerIdeApi(
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IIdeComponentRenderers ideComponentRenderers,
        ICommonComponentRenderers commonComponentRenderers,
        ITreeViewService treeViewService,
        BackgroundTaskService backgroundTaskService,
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

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly ConcurrentQueue<FolderExplorerIdeApiWorkArgs> _workKindQueue = new();

    public void Enqueue(FolderExplorerIdeApiWorkArgs workArgs)
    {
        _workKindQueue.Enqueue(workArgs);
        _backgroundTaskService.Continuous_EnqueueGroup(this);
    }

    private ValueTask Do_SetFolderExplorerState(AbsolutePath folderAbsolutePath)
    {
        _folderExplorerService.With(
            inFolderExplorerState => inFolderExplorerState with
            {
                AbsolutePath = folderAbsolutePath
            });

        return Do_SetFolderExplorerTreeView(folderAbsolutePath);
    }

    private async ValueTask Do_SetFolderExplorerTreeView(AbsolutePath folderAbsolutePath)
    {
        _folderExplorerService.With(inFolderExplorerState => inFolderExplorerState with
        {
            IsLoadingFolderExplorer = true
        });
        
		_environmentProvider.DeletionPermittedRegister(new(folderAbsolutePath.Value, true));

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
                new List<TreeViewNoType>() { rootNode }));
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

        _folderExplorerService.With(inFolderExplorerState => inFolderExplorerState with
        {
            IsLoadingFolderExplorer = false
        });
    }

    public void ShowInputFile()
    {
        _ideBackgroundTaskApi.InputFile.Enqueue_RequestInputFileStateForm(
            "Folder Explorer",
            async absolutePath =>
            {
                if (absolutePath.ExactInput is not null)
                    await Do_SetFolderExplorerState(absolutePath).ConfigureAwait(false);
            },
            absolutePath =>
            {
                if (absolutePath.ExactInput is null || !absolutePath.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            },
            [
                new InputFilePattern("Directory", absolutePath => absolutePath.IsDirectory)
            ]);
    }

    public ValueTask HandleEvent()
    {
        if (!_workKindQueue.TryDequeue(out FolderExplorerIdeApiWorkArgs workArgs))
            return ValueTask.CompletedTask;

        switch (workArgs.FolderExplorerIdeApiWorkKind)
        {
            case FolderExplorerIdeApiWorkKind.SetFolderExplorerState:
                return Do_SetFolderExplorerState(workArgs.AbsolutePath);
            case FolderExplorerIdeApiWorkKind.SetFolderExplorerTreeView:
                return Do_SetFolderExplorerTreeView(workArgs.AbsolutePath);
            default:
                Console.WriteLine($"{nameof(FolderExplorerIdeApi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
        }
    }
}
