using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitDiffDisplay : ComponentBase
{
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;
    [Inject]
    private LuthetusIdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;

    [Parameter, EditorRequired]
    public GitFile GitFile { get; set; } = null!;

    public Key<TerminalCommand> GitLogTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();

    private async Task ShowOriginalFromGitOnClick(GitState localGitState, GitFile localGitFile)
    {
        if (localGitState.Repo is null)
            return;

        await IdeBackgroundTaskApi.Git.LogFileEnqueue(localGitState.Repo, localGitFile.RelativePathString)
            .ConfigureAwait(false);
    }
}