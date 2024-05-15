using Fluxor;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitControlsDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
    private LuthetusIdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;

    [CascadingParameter]
    public GitState GitState { get; set; } = null!;

    private string _summary = string.Empty;

    private async Task ExecuteGitStatusTerminalCommandOnClick()
    {
		await IdeBackgroundTaskApi.Git.GitStatusExecute()
			.ConfigureAwait(false);
    }

    private async Task StageOnClick(GitState localGitState)
    {
        if (localGitState.Repo is null)
            return;

        await IdeBackgroundTaskApi.Git.GitAddExecute(localGitState.Repo)
			.ConfigureAwait(false);
    }

    private async Task UnstageOnClick(GitState localGitState)
    {
        if (localGitState.Repo is null)
            return;

        await IdeBackgroundTaskApi.Git.GitUnstageExecute(localGitState.Repo)
            .ConfigureAwait(false);
    }

    private async Task CommitChangesOnClick(GitState localGitState, string localSummary)
    {
        if (localGitState.Repo is null)
            return;

        await IdeBackgroundTaskApi.Git.GitCommitExecute(localGitState.Repo, localSummary)
            .ConfigureAwait(false);

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
            true);

        Dispatcher.Dispatch(new DialogState.RegisterAction(dialogViewModel));
    }
}