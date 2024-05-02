using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Gits.Models;

public class GitTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly IState<GitState> _gitStateWrap;
    private readonly IDispatcher _dispatcher;

    public GitTreeViewKeyboardEventHandler(
            ITreeViewService treeViewService,
            IBackgroundTaskService backgroundTaskService,
            IState<GitState> gitStateWrap,
            IDispatcher dispatcher)
        : base(treeViewService, backgroundTaskService)
    {
        _gitStateWrap = gitStateWrap;
        _dispatcher = dispatcher;
    }

    public override void OnKeyDown(TreeViewCommandArgs commandArgs)
    {
        if (commandArgs.KeyboardEventArgs is null)
            return;

        if (commandArgs.KeyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE ||
            commandArgs.KeyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            var localGitState = _gitStateWrap.Value;

            _dispatcher.Dispatch(new GitState.SetGitStateWithAction(inState =>
            {
                if (inState.GitFolderAbsolutePath != localGitState.GitFolderAbsolutePath)
                {
                    // Git folder was changed, throw away the result since it is thereby invalid.
                    return inState;
                }

                var outStagedGitFileMap = new Dictionary<string, GitFile>(inState.StagedGitFileMap);

                foreach (var selectedNode in commandArgs.TreeViewContainer.SelectedNodeList)
                {
                    if (selectedNode is TreeViewGitFile treeViewGitFile)
                    {
                        var key = treeViewGitFile.Item.AbsolutePath.Value;

                        var wasRemoved = outStagedGitFileMap.Remove(key);
                        if (!wasRemoved)
                            outStagedGitFileMap.Add(key, treeViewGitFile.Item);
                    }
                }

                return inState with
                {
                    StagedGitFileMap = outStagedGitFileMap.ToImmutableDictionary()
                };
            }));
        }

        base.OnKeyDown(commandArgs);
    }
}
