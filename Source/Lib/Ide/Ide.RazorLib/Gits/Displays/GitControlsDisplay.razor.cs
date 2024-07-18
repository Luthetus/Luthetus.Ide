using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitControlsDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
    private IdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;

    [CascadingParameter]
    public GitState GitState { get; set; } = null!;

    private string _summary = string.Empty;

    private void ExecuteGitRefreshOnClick(GitState localGitState)
    {
        if (localGitState.Repo is null)
            return;

        IdeBackgroundTaskApi.Git.RefreshEnqueue(localGitState.Repo);
    }

    private void StageOnClick(GitState localGitState)
    {
        if (localGitState.Repo is null)
            return;

        IdeBackgroundTaskApi.Git.AddEnqueue(localGitState.Repo);
    }

    private void UnstageOnClick(GitState localGitState)
    {
        if (localGitState.Repo is null)
            return;

        IdeBackgroundTaskApi.Git.UnstageEnqueue(localGitState.Repo);
    }

    private void CommitChangesOnClick(GitState localGitState, string localSummary)
    {
        if (localGitState.Repo is null)
            return;

        IdeBackgroundTaskApi.Git.CommitEnqueue(localGitState.Repo, localSummary);

        _summary = string.Empty;
    }

    private void ShowGitOriginDialogOnClick()
    {
        var dialogViewModel = new DialogViewModel(
            Key<IDynamicViewModel>.NewKey(),
            $"Git Origin",
            typeof(GitOriginDisplay),
            null,
            null,
            true,
            null);

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogViewModel));
    }
}