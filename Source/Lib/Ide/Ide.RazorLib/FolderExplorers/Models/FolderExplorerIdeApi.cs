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
    public Key<BackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public string Name { get; } = nameof(FolderExplorerIdeApi);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<FolderExplorerIdeApiWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();

    private readonly Queue<AbsolutePath> queue_general_AbsolutePath = new();

    public void Enqueue_SetFolderExplorerState(AbsolutePath folderAbsolutePath)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(FolderExplorerIdeApiWorkKind.SetFolderExplorerState);
            queue_general_AbsolutePath.Enqueue(folderAbsolutePath);
            _backgroundTaskService.EnqueueGroup(this);
        }
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

    public void Enqueue_SetFolderExplorerTreeView(AbsolutePath folderAbsolutePath)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(FolderExplorerIdeApiWorkKind.SetFolderExplorerTreeView);
            queue_general_AbsolutePath.Enqueue(folderAbsolutePath);
            _backgroundTaskService.EnqueueGroup(this);
        }
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

    public IBackgroundTaskGroup? EarlyBatchOrDefault(IBackgroundTaskGroup oldEvent)
    {
        return null;
    }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        FolderExplorerIdeApiWorkKind workKind;

        lock (_workLock)
        {
            if (!_workKindQueue.TryDequeue(out workKind))
                return ValueTask.CompletedTask;
        }

        switch (workKind)
        {
            case FolderExplorerIdeApiWorkKind.SetFolderExplorerState:
            {
                var args = queue_general_AbsolutePath.Dequeue();
                return Do_SetFolderExplorerState(args);
            }
            case FolderExplorerIdeApiWorkKind.SetFolderExplorerTreeView:
            {
                var args = queue_general_AbsolutePath.Dequeue();
                return Do_SetFolderExplorerTreeView(args);
            }
            default:
            {
                Console.WriteLine($"{nameof(FolderExplorerIdeApi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
            }
        }
    }
}
