using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Extensions.Git.CommandLines.Models;
using Luthetus.Extensions.Git.Models;
using Luthetus.Extensions.Git.BackgroundTasks.Models;

namespace Luthetus.Extensions.Git.Displays;

public partial class GitOriginDisplay : ComponentBase
{
    [Inject]
    private ITerminalService TerminalService { get; set; } = null!;
    [Inject]
    private GitBackgroundTaskApi GitBackgroundTaskApi { get; set; } = null!;

    private string _gitOrigin = string.Empty;
    
    private string CommandArgs => $"remote add origin \"{_gitOrigin}\"";
    
    public Key<TerminalCommandRequest> GitSetOriginTerminalCommandRequestKey { get; } = Key<TerminalCommandRequest>.NewKey();

    private void GetOriginOnClick()
    {
        var localGitState = GitBackgroundTaskApi.Git.GetGitState();

        if (localGitState.Repo is null)
            return;
        
        GitBackgroundTaskApi.Git.Enqueue(new GitIdeApiWorkArgs
        {
        	WorkKind = GitIdeApiWorkKind.GetOriginName,
        	RepoAtTimeOfRequest = localGitState.Repo
    	});
    }

    private void SetGitOriginOnClick(string localCommandArgs)
    {
        var localGitState = GitBackgroundTaskApi.Git.GetGitState();

        if (localGitState.Repo is null)
            return;

        var formattedCommand = new FormattedCommand(
            GitCliFacts.TARGET_FILE_NAME,
            new string[] { localCommandArgs })
        {
            HACK_ArgumentsString = localCommandArgs,
            Tag = GitCliOutputParser.TagConstants.SetGitOrigin,
        };

        var terminalCommandRequest = new TerminalCommandRequest(
        	formattedCommand.Value,
        	localGitState.Repo.AbsolutePath.Value,
        	GitSetOriginTerminalCommandRequestKey);
        	
        TerminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
    }
}