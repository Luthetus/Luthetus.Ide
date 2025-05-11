using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Extensions.Git.Models;

public class GitTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
    private readonly GitIdeApi _gitIdeApi;

    public GitTreeViewKeyboardEventHandler(
            ITreeViewService treeViewService,
            BackgroundTaskService backgroundTaskService,
            GitIdeApi gitIdeApi)
        : base(treeViewService, backgroundTaskService)
    {
        _gitIdeApi = gitIdeApi;
    }

    protected override void OnKeyDown(TreeViewCommandArgs commandArgs)
    {
        if (commandArgs.KeyboardEventArgs is null)
            return;

        if (commandArgs.KeyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE ||
            commandArgs.KeyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            var localGitState = _gitIdeApi.GetGitState();

            _gitIdeApi.ReduceSetGitStateWithAction(inState =>
            {
                if (inState.Repo != localGitState.Repo)
                {
                    // Git folder was changed, throw away the result since it is thereby invalid.
                    return inState;
                }

                var outSelectedFileList = new List<GitFile>(inState.SelectedFileList);

                foreach (var selectedNode in commandArgs.TreeViewContainer.SelectedNodeList)
                {
                    if (selectedNode is TreeViewGitFile treeViewGitFile)
                    {
                        var key = treeViewGitFile.Item.AbsolutePath.Value;

                        var indexOf = outSelectedFileList.FindIndex(x => x.AbsolutePath.Value == key);

                        // Toggle
                        if (indexOf != -1)
                            outSelectedFileList.RemoveAt(indexOf);
                        else
                            outSelectedFileList.Add(treeViewGitFile.Item);
                    }
                }

                return inState with
                {
                    SelectedFileList = outSelectedFileList
                };
            });
        }

        base.OnKeyDown(commandArgs);
    }
}
