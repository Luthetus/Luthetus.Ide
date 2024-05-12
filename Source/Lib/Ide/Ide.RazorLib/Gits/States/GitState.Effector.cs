using Fluxor;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Gits.States;

public partial record GitState
{
	public class Effector
	{
        private readonly IState<GitState> _gitStateWrap;
        private readonly ILuthetusIdeComponentRenderers _ideComponentRenderers;
        private readonly ITreeViewService _treeViewService;
        private readonly ThrottleAsync _throttle = new ThrottleAsync(TimeSpan.FromMilliseconds(300));

        public Effector(
            IState<GitState> gitStateWrap,
            ILuthetusIdeComponentRenderers ideComponentRenderers,
            ITreeViewService treeViewService)
        {
            _gitStateWrap = gitStateWrap;
            _ideComponentRenderers = ideComponentRenderers;
            _treeViewService = treeViewService;
        }

        [EffectMethod(typeof(SetFileListAction))]
		public Task HandleSetGitStateWithAction(IDispatcher dispatcher)
		{
            // Suppress unused variable warning
            _ = dispatcher;

            return _throttle.PushEvent(_ =>
            {
                var gitState = _gitStateWrap.Value;

                var treeViewList = gitState.FileList.Select(x => new TreeViewGitFile(
                        x,
                        _ideComponentRenderers,
                        false,
                        false))
                    .ToArray();

                var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(treeViewList);
                var firstNode = treeViewList.FirstOrDefault();

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
        }
    }
}
