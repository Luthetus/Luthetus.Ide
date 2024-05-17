using Fluxor;
using System.Text;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Ide.RazorLib.Gits.Models;

public class LuthetusIdeGitBackgroundTaskApi
{
    private readonly LuthetusIdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IState<TerminalState> _terminalStateWrap;
    private readonly IState<GitState> _gitStateWrap;
    private readonly IEnvironmentProvider _environmentProvider;
	private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly IDispatcher _dispatcher;

    public LuthetusIdeGitBackgroundTaskApi(
		LuthetusIdeBackgroundTaskApi ideBackgroundTaskApi,
        IState<TerminalState> terminalStateWrap,
        IState<GitState> gitStateWrap,
        IEnvironmentProvider environmentProvider,
		IBackgroundTaskService backgroundTaskService,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        IDispatcher dispatcher)
    {
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
        _terminalStateWrap = terminalStateWrap;
		_gitStateWrap = gitStateWrap;
		_environmentProvider = environmentProvider;
		_backgroundTaskService = backgroundTaskService;
        _commonComponentRenderers = commonComponentRenderers;
        _dispatcher = dispatcher;
    }

    public Key<TerminalCommand> GitTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();

    public async Task StatusEnqueue()
    {
        await _backgroundTaskService.EnqueueAsync(
            Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git status -u",
            async () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null)
                    return;

                var gitStatusDashUCommand = $"{GitCliFacts.STATUS_COMMAND} -u";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { gitStatusDashUCommand })
                {
                    HACK_ArgumentsString = gitStatusDashUCommand
                };

                var gitCliOutputParser = new GitCliOutputParser(
                    _dispatcher,
                    localGitState,
                    _environmentProvider,
                    GitCliOutputParser.GitCommandKind.Status);

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: gitCliOutputParser);

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                await generalTerminal
                    .EnqueueCommandAsync(terminalCommand)
                    .ConfigureAwait(false);
            });
    }

    public async Task RefreshEnqueue(GitRepo repoAtTimeOfRequest)
    {
		await StatusEnqueue();
        await GetActiveBranchNameEnqueue(repoAtTimeOfRequest);
        await GetOriginNameEnqueue(repoAtTimeOfRequest);
    }

    public async Task GetActiveBranchNameEnqueue(GitRepo repoAtTimeOfRequest)
    {
        await _backgroundTaskService.EnqueueAsync(
            Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git get active branch name",
            async () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
                    return;

                var terminalCommandArgs = $"branch --show-current";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { terminalCommandArgs })
                {
                    HACK_ArgumentsString = terminalCommandArgs
                };

                var gitCliOutputParser = new GitCliOutputParser(
                    _dispatcher,
                    localGitState,
                    _environmentProvider,
                    GitCliOutputParser.GitCommandKind.GetBranch);

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: gitCliOutputParser);

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                await generalTerminal
                    .EnqueueCommandAsync(terminalCommand)
                    .ConfigureAwait(false);
            });
    }

    public async Task GetOriginNameEnqueue(GitRepo repoAtTimeOfRequest)
    {
        await _backgroundTaskService.EnqueueAsync(
            Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git get origin name",
            async () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
                    return;

                var terminalCommandArgs = $"config --get remote.origin.url";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { terminalCommandArgs })
                {
                    HACK_ArgumentsString = terminalCommandArgs
                };

                var gitCliOutputParser = new GitCliOutputParser(
                    _dispatcher,
                    localGitState,
                    _environmentProvider,
                    GitCliOutputParser.GitCommandKind.GetOrigin);

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: gitCliOutputParser);

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                await generalTerminal
                    .EnqueueCommandAsync(terminalCommand)
                    .ConfigureAwait(false);
            });
    }

    public async Task AddEnqueue(GitRepo repoAtTimeOfRequest)
    {
		await _backgroundTaskService.EnqueueAsync(
			Key<BackgroundTask>.NewKey(),
			ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git add",
            async () =>
			{
				var localGitState = _gitStateWrap.Value;

		        if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
		            return;
		
		        var filesBuilder =  new StringBuilder();
		
		        foreach (var fileAbsolutePath in localGitState.SelectedFileList)
		        {
		            var relativePathString = PathHelper.GetRelativeFromTwoAbsolutes(
		                localGitState.Repo.AbsolutePath,
		                fileAbsolutePath.AbsolutePath,
		                _environmentProvider);
		
		            if (_environmentProvider.DirectorySeparatorChar == '\\')
		            {
                        // The following fails (directory separator character):
                        //     git add ".\MyApp\"
                        //
                        // Whereas the following succeeds
                        //     git add "./MyApp/"
                        relativePathString = relativePathString.Replace(
		                    _environmentProvider.DirectorySeparatorChar,
		                    _environmentProvider.AltDirectorySeparatorChar);
		            }
		
		            filesBuilder.Append($"\"{relativePathString}\" ");
		        }
		
		        var terminalCommandArgs = "add " + filesBuilder.ToString();
		
		        var formattedCommand = new FormattedCommand(
		            GitCliFacts.TARGET_FILE_NAME,
		            new string[] { terminalCommandArgs })
		        {
		            HACK_ArgumentsString = terminalCommandArgs
		        };
		        
		        var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
		            formattedCommand,
		            localGitState.Repo.AbsolutePath.Value,
                    ContinueWith: StatusEnqueue);
		
		        var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
		        await generalTerminal
		            .EnqueueCommandAsync(terminalCommand)
		            .ConfigureAwait(false);
			});
    }
	
	public async Task UnstageEnqueue(GitRepo repoAtTimeOfRequest)
    {
		await _backgroundTaskService.EnqueueAsync(
			Key<BackgroundTask>.NewKey(),
			ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git unstage",
            async () =>
			{
				var localGitState = _gitStateWrap.Value;

		        if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
		            return;
		
		        var filesBuilder =  new StringBuilder();
		
		        foreach (var fileAbsolutePath in localGitState.SelectedFileList)
		        {
		            var relativePathString = PathHelper.GetRelativeFromTwoAbsolutes(
		                localGitState.Repo.AbsolutePath,
		                fileAbsolutePath.AbsolutePath,
		                _environmentProvider);
		
		            if (_environmentProvider.DirectorySeparatorChar == '\\')
		            {
                        // The following fails (directory separator character):
                        //     git restore --staged ".\MyApp\"
                        //
                        // Whereas the following succeeds
                        //     git restore --staged "./MyApp/"
                        relativePathString = relativePathString.Replace(
		                    _environmentProvider.DirectorySeparatorChar,
		                    _environmentProvider.AltDirectorySeparatorChar);
		            }
		
		            filesBuilder.Append($"\"{relativePathString}\" ");
		        }
		
		        var terminalCommandArgs = "restore --staged " + filesBuilder.ToString();
		
		        var formattedCommand = new FormattedCommand(
		            GitCliFacts.TARGET_FILE_NAME,
		            new string[] { terminalCommandArgs })
		        {
		            HACK_ArgumentsString = terminalCommandArgs
		        };
		        
		        var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
		            formattedCommand,
		            localGitState.Repo.AbsolutePath.Value,
                    ContinueWith: StatusEnqueue);
		
		        var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
		        await generalTerminal
		            .EnqueueCommandAsync(terminalCommand)
		            .ConfigureAwait(false);
			});
    }
	
	public async Task CommitEnqueue(GitRepo repoAtTimeOfRequest, string commitSummary)
    {
		await _backgroundTaskService.EnqueueAsync(
			Key<BackgroundTask>.NewKey(),
			ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git commit",
            async () =>
			{
				var localGitState = _gitStateWrap.Value;

		        if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
		            return;

                var argumentsString = $"commit -m \"{commitSummary}\"";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString
                };

                var gitCommitCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    ContinueWith: async () =>
					{
						await StatusEnqueue();

						NotificationHelper.DispatchInformative(
							"Git: committed",
                            commitSummary,
							_commonComponentRenderers,
							_dispatcher,
							TimeSpan.FromSeconds(5));
                    });

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                await generalTerminal
                    .EnqueueCommandAsync(gitCommitCommand)
                    .ConfigureAwait(false);
            });
    }

    public async Task BranchNewEnqueue(GitRepo repoAtTimeOfRequest, string branchName)
    {
        if (string.IsNullOrWhiteSpace(branchName))
            NotificationHelper.DispatchError(nameof(BranchNewEnqueue), "branchName was null or whitespace", _commonComponentRenderers, _dispatcher, TimeSpan.FromSeconds(6));

        await _backgroundTaskService.EnqueueAsync(
            Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git new branch",
            async () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
                    return;

                var argumentsString = "checkout -b " + branchName;

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString
                };

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    ContinueWith: () => RefreshEnqueue(repoAtTimeOfRequest));

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                await generalTerminal
                    .EnqueueCommandAsync(terminalCommand)
                    .ConfigureAwait(false);
            });
    }

    public async Task BranchGetAllEnqueue(GitRepo repoAtTimeOfRequest)
    {
        await _backgroundTaskService.EnqueueAsync(
            Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git branch -a",
            async () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
                    return;

                var argumentsString = "branch -a";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString
                };

                var gitCliOutputParser = new GitCliOutputParser(
                    _dispatcher,
                    localGitState,
                    _environmentProvider,
                    GitCliOutputParser.GitCommandKind.GetBranchList);

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: gitCliOutputParser);

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                await generalTerminal
                    .EnqueueCommandAsync(terminalCommand)
                    .ConfigureAwait(false);
            });
    }
    
    public async Task BranchSetEnqueue(GitRepo repoAtTimeOfRequest, string branchName)
    {
        await _backgroundTaskService.EnqueueAsync(
            Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            $"git checkout {branchName}",
            async () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
                    return;

                var argumentsString = $"checkout {branchName}";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString
                };

                var gitCliOutputParser = new GitCliOutputParser(
                    _dispatcher,
                    localGitState,
                    _environmentProvider,
                    GitCliOutputParser.GitCommandKind.GetBranchList);

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: gitCliOutputParser,
                    ContinueWith: () => RefreshEnqueue(repoAtTimeOfRequest));

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                await generalTerminal
                    .EnqueueCommandAsync(terminalCommand)
                    .ConfigureAwait(false);
            });
    }
    
    public async Task PushToOriginWithTrackingEnqueue(GitRepo repoAtTimeOfRequest)
    {
        await _backgroundTaskService.EnqueueAsync(
            Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git push -u origin {branchName will go here}",
            async () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
                    return;

                // This command will push to origin, and then set the upstream variable
                // (unsure if this is the correct description)
                var argumentsString = $"push -u origin {localGitState.Branch}";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString
                };

                var gitCliOutputParser = new GitCliOutputParser(
                    _dispatcher,
                    localGitState,
                    _environmentProvider,
                    GitCliOutputParser.GitCommandKind.PushToOriginWithTracking);

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: gitCliOutputParser,
                    ContinueWith: () => RefreshEnqueue(repoAtTimeOfRequest));

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                await generalTerminal
                    .EnqueueCommandAsync(terminalCommand)
                    .ConfigureAwait(false);
            });
    }

    public async Task PullEnqueue(GitRepo repoAtTimeOfRequest)
    {
        await _backgroundTaskService.EnqueueAsync(
            Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git pull",
            async () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
                    return;

                var argumentsString = $"pull";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString
                };

                var gitCliOutputParser = new GitCliOutputParser(
                    _dispatcher,
                    localGitState,
                    _environmentProvider,
                    GitCliOutputParser.GitCommandKind.Pull);

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: gitCliOutputParser,
                    ContinueWith: () => RefreshEnqueue(repoAtTimeOfRequest));

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                await generalTerminal
                    .EnqueueCommandAsync(terminalCommand)
                    .ConfigureAwait(false);
            });
    }
    
    public async Task FetchEnqueue(GitRepo repoAtTimeOfRequest)
    {
        await _backgroundTaskService.EnqueueAsync(
            Key<BackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git fetch",
            async () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
                    return;

                var argumentsString = $"fetch";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString
                };

                var gitCliOutputParser = new GitCliOutputParser(
                    _dispatcher,
                    localGitState,
                    _environmentProvider,
                    GitCliOutputParser.GitCommandKind.Pull);

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: gitCliOutputParser,
                    ContinueWith: () => RefreshEnqueue(repoAtTimeOfRequest));

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                await generalTerminal
                    .EnqueueCommandAsync(terminalCommand)
                    .ConfigureAwait(false);
            });
    }
}
