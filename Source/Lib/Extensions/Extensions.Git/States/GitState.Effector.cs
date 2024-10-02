using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.Git.Models;
using Luthetus.Extensions.Git.BackgroundTasks.Models;
using Luthetus.Extensions.Git.ComponentRenderers.Models;

namespace Luthetus.Extensions.Git.States;

public partial record GitState
{
	public class Effector
	{
        private readonly IState<GitState> _gitStateWrap;
        private readonly GitTreeViews _gitTreeViews;
        private readonly IIdeComponentRenderers _ideComponentRenderers;
        private readonly ICommonComponentRenderers _commonComponentRenderers;
        private readonly ITreeViewService _treeViewService;
        private readonly GitBackgroundTaskApi _gitBackgroundTaskApi;
        private readonly Throttle _throttle = new(TimeSpan.FromMilliseconds(300));

        public Effector(
            IState<GitState> gitStateWrap,
            GitTreeViews gitTreeViews,
            IIdeComponentRenderers ideComponentRenderers,
            ICommonComponentRenderers commonComponentRenderers,
            ITreeViewService treeViewService,
            GitBackgroundTaskApi gitBackgroundTaskApi)
        {
            _gitStateWrap = gitStateWrap;
            _gitTreeViews = gitTreeViews;
            _ideComponentRenderers = ideComponentRenderers;
            _commonComponentRenderers = commonComponentRenderers;
            _treeViewService = treeViewService;
            _gitBackgroundTaskApi = gitBackgroundTaskApi;
        }

        [EffectMethod(typeof(SetStatusAction))]
		public Task HandleSetFileListAction(IDispatcher dispatcher)
		{
            // Suppress unused variable warning
            _ = dispatcher;

			_throttle.Run(_ =>
            {
                var gitState = _gitStateWrap.Value;

                var untrackedTreeViewList = gitState.UntrackedFileList.Select(x => new TreeViewGitFile(
                        x,
                        _gitTreeViews,
                        false,
                        false))
                    .ToArray();

                var untrackedFileGroupTreeView = new TreeViewGitFileGroup(
                    "Untracked",
                    _ideComponentRenderers,
                    _commonComponentRenderers,
                    true,
                    true);

                untrackedFileGroupTreeView.SetChildList(untrackedTreeViewList);

                var stagedTreeViewList = gitState.StagedFileList.Select(x => new TreeViewGitFile(
                        x,
                        _gitTreeViews,
                        false,
                        false))
                    .ToArray();

                var stagedFileGroupTreeView = new TreeViewGitFileGroup(
                    "Staged",
                    _ideComponentRenderers,
                    _commonComponentRenderers,
                    true,
                    true);

                stagedFileGroupTreeView.SetChildList(stagedTreeViewList);
                
                var unstagedTreeViewList = gitState.UnstagedFileList.Select(x => new TreeViewGitFile(
                        x,
                        _gitTreeViews,
                        false,
                        false))
                    .ToArray();

                var unstagedFileGroupTreeView = new TreeViewGitFileGroup(
                    "Not-staged",
                    _ideComponentRenderers,
                    _commonComponentRenderers,
                    true,
                    true);

                unstagedFileGroupTreeView.SetChildList(unstagedTreeViewList);

                var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(
                    stagedFileGroupTreeView,
                    unstagedFileGroupTreeView,
                    untrackedFileGroupTreeView);

                var firstNode = untrackedTreeViewList.FirstOrDefault();

                var activeNodes = firstNode is null
                    ? Array.Empty<TreeViewNoType>()
                    : new[] { firstNode };

                if (!_treeViewService.TryGetTreeViewContainer(TreeViewGitChangesKey, out var container))
                {
                    _treeViewService.RegisterTreeViewContainer(new TreeViewContainer(
                        TreeViewGitChangesKey,
                        adhocRoot,
                        activeNodes.ToImmutableList()));
                }
                else
                {
                    _treeViewService.SetRoot(TreeViewGitChangesKey, adhocRoot);

                    _treeViewService.SetActiveNode(
                        TreeViewGitChangesKey,
                        firstNode,
                        true,
                        false);
                }

                return Task.CompletedTask;
            });
            
            return Task.CompletedTask;
        }
        
        [EffectMethod]
		public Task HandleSetRepoAction(
            SetRepoAction setRepoAction,
            IDispatcher dispatcher)
		{
            // Suppress unused variable warning
            _ = dispatcher;

            if (setRepoAction.Repo is not null)
                _gitBackgroundTaskApi.Git.RefreshEnqueue(setRepoAction.Repo);

            return Task.CompletedTask;
        }
    }
}
