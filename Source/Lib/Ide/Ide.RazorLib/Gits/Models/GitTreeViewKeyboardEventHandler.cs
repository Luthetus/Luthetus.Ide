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

    protected override void OnKeyDown(TreeViewCommandArgs commandArgs)
    {
        if (commandArgs.KeyboardEventArgs is null)
            return;

        if (commandArgs.KeyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE ||
            commandArgs.KeyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            var localGitState = _gitStateWrap.Value;

            _dispatcher.Dispatch(new GitState.WithAction(inState =>
            {
                if (inState.Repo != localGitState.Repo)
                {
                    // Git folder was changed, throw away the result since it is thereby invalid.
                    return inState;
                }

                var outStagedFileList = new List<GitFile>(inState.StagedFileList);

                foreach (var selectedNode in commandArgs.TreeViewContainer.SelectedNodeList)
                {
                    if (selectedNode is TreeViewGitFile treeViewGitFile)
                    {
                        var key = treeViewGitFile.Item.AbsolutePath.Value;

                        var indexOf = outStagedFileList.FindIndex(x => x.AbsolutePath.Value == key);

                        // Toggle
                        if (indexOf != -1)
                            outStagedFileList.RemoveAt(indexOf);
                        else
                            outStagedFileList.Add(treeViewGitFile.Item);
                    }
                }

                return inState with
                {
                    StagedFileList = outStagedFileList.ToImmutableList()
                };
            }));
        }

        base.OnKeyDown(commandArgs);
    }
}
