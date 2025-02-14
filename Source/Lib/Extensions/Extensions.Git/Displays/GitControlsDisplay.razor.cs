using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Extensions.Git.Models;
using Luthetus.Extensions.Git.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Extensions.Git.Displays;

public partial class GitControlsDisplay : ComponentBase
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;
	[Inject]
    private GitBackgroundTaskApi GitBackgroundTaskApi { get; set; } = null!;

    [CascadingParameter]
    public GitState GitState { get; set; } = null!;

    private string _summary = string.Empty;

    private void ExecuteGitRefreshOnClick(GitState localGitState)
    {
        if (localGitState.Repo is null)
            return;

        GitBackgroundTaskApi.Git.RefreshEnqueue(localGitState.Repo);
    }

    private void StageOnClick(GitState localGitState)
    {
        if (localGitState.Repo is null)
            return;

        GitBackgroundTaskApi.Git.AddEnqueue(localGitState.Repo);
    }

    private void UnstageOnClick(GitState localGitState)
    {
        if (localGitState.Repo is null)
            return;

        GitBackgroundTaskApi.Git.UnstageEnqueue(localGitState.Repo);
    }

    private void CommitChangesOnClick(GitState localGitState, string localSummary)
    {
        if (localGitState.Repo is null)
            return;

        GitBackgroundTaskApi.Git.CommitEnqueue(localGitState.Repo, localSummary);

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

        CommonApi.DialogApi.ReduceRegisterAction(dialogViewModel);
    }
}