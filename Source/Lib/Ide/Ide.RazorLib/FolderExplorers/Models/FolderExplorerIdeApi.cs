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
	private readonly LuthetusCommonApi _commonApi;
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly IFolderExplorerService _folderExplorerService;

    public FolderExplorerIdeApi(
    	LuthetusCommonApi commonApi,
        IdeBackgroundTaskApi ideBackgroundTaskApi,
        IIdeComponentRenderers ideComponentRenderers,
        IFolderExplorerService folderExplorerService)
    {
    	_commonApi = commonApi;
        _ideBackgroundTaskApi = ideBackgroundTaskApi;
        _ideComponentRenderers = ideComponentRenderers;
        _folderExplorerService = folderExplorerService;
    }

    public void SetFolderExplorerState(AbsolutePath folderAbsolutePath)
    {
        _commonApi.BackgroundTaskApi.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            BackgroundTaskFacts.ContinuousQueueKey,
            "Set FolderExplorer State",
            async () => await SetFolderExplorerAsync(folderAbsolutePath).ConfigureAwait(false));
    }

    public void SetFolderExplorerTreeView(AbsolutePath folderAbsolutePath)
    {
        _commonApi.BackgroundTaskApi.Enqueue(
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
            _commonApi,
            _ideComponentRenderers,
            true,
            true);

        await rootNode.LoadChildListAsync().ConfigureAwait(false);

        if (!_commonApi.TreeViewApi.TryGetTreeViewContainer(
                FolderExplorerState.TreeViewContentStateKey,
                out var treeViewState))
        {
            _commonApi.TreeViewApi.ReduceRegisterContainerAction(new TreeViewContainer(
                FolderExplorerState.TreeViewContentStateKey,
                rootNode,
                new() { rootNode }));
        }
        else
        {
            _commonApi.TreeViewApi.ReduceWithRootNodeAction(FolderExplorerState.TreeViewContentStateKey, rootNode);

            _commonApi.TreeViewApi.ReduceSetActiveNodeAction(
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
