using Fluxor;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Gits.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;

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
    private IdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private GitCliOutputParser GitCliOutputParser { get; set; } = null!;

    private string _gitOrigin = string.Empty;
    
    private string CommandArgs => $"remote add origin \"{_gitOrigin}\"";
    
    public Key<TerminalCommand> GitSetOriginTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();

    private void GetOriginOnClick()
    {
        var localGitState = GitStateWrap.Value;

        if (localGitState.Repo is null)
            return;
        
        IdeBackgroundTaskApi.Git.GetOriginNameEnqueue(localGitState.Repo);
    }

    private void SetGitOriginOnClick(string localCommandArgs)
    {
        var localGitState = GitStateWrap.Value;

        if (localGitState.Repo is null)
            return;

        var formattedCommand = new FormattedCommand(
            GitCliFacts.TARGET_FILE_NAME,
            new string[] { localCommandArgs })
        {
            HACK_ArgumentsString = localCommandArgs,
            Tag = GitCliOutputParser.TagConstants.SetGitOrigin,
        };

        var gitStatusCommand = new TerminalCommand(
            GitSetOriginTerminalCommandKey,
            formattedCommand,
            localGitState.Repo.AbsolutePath.Value,
            OutputParser: GitCliOutputParser);

        var terminalCommandRequest = new TerminalCommandRequest(
        	formattedCommand.Value,
        	localGitState.Repo.AbsolutePath.Value,
        	new Key<TerminalCommandRequest>(GitSetOriginTerminalCommandKey.Guid));
        	
        TerminalStateWrap.Value.NEW_TERMINAL.EnqueueCommand(terminalCommandRequest);
    }
}