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

public partial class GitOriginDisplay : ComponentBase
{
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;
    [Inject]
    private IState<TerminalState> TerminalStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private LuthetusIdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;

    private string _gitOrigin = string.Empty;
    
    private string CommandArgs => $"remote add origin \"{_gitOrigin}\"";
    
    public Key<TerminalCommand> GitSetOriginTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();

    private async Task GetOriginOnClick()
    {
        var localGitState = GitStateWrap.Value;

        if (localGitState.Repo is null)
            return;
        
        await IdeBackgroundTaskApi.Git.GetOriginNameEnqueue(localGitState.Repo)
            .ConfigureAwait(false);
    }

    private async Task SetGitOriginOnClick(string localCommandArgs)
    {
        var localGitState = GitStateWrap.Value;

        if (localGitState.Repo is null)
            return;

        var formattedCommand = new FormattedCommand(
            GitCliFacts.TARGET_FILE_NAME,
            new string[] { localCommandArgs })
        {
            HACK_ArgumentsString = localCommandArgs
        };

        var gitCliOutputParser = new GitCliOutputParser(
            Dispatcher,
            localGitState,
            EnvironmentProvider,
            GitCliOutputParser.GitCommandKind.None);

        var gitStatusCommand = new TerminalCommand(
            GitSetOriginTerminalCommandKey,
            formattedCommand,
            localGitState.Repo.AbsolutePath.Value,
            OutputParser: gitCliOutputParser);

        var generalTerminal = TerminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
        await generalTerminal
            .EnqueueCommandAsync(gitStatusCommand)
            .ConfigureAwait(false);
    }
}