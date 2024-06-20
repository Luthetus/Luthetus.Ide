using Fluxor;
using System.Text;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Luthetus.Ide.RazorLib.CommandLines.Models;

namespace Luthetus.Ide.RazorLib.Gits.Models;

public class LuthetusIdeGitBackgroundTaskApi
{
    private readonly LuthetusIdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IState<TerminalState> _terminalStateWrap;
    private readonly IState<GitState> _gitStateWrap;
	private readonly GitCliOutputParser _gitCliOutputParser;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly IDispatcher _dispatcher;

    public LuthetusIdeGitBackgroundTaskApi(
		LuthetusIdeBackgroundTaskApi ideBackgroundTaskApi,
        IState<TerminalState> terminalStateWrap,
        IState<GitState> gitStateWrap,
		GitCliOutputParser gitCliOutputParser,
		IEnvironmentProvider environmentProvider,
		IBackgroundTaskService backgroundTaskService,
        ILuthetusCommonComponentRenderers commonComponentRenderers,
        IDispatcher dispatcher)
    {
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
        _terminalStateWrap = terminalStateWrap;
		_gitStateWrap = gitStateWrap;
		_gitCliOutputParser = gitCliOutputParser;
		_environmentProvider = environmentProvider;
		_backgroundTaskService = backgroundTaskService;
        _commonComponentRenderers = commonComponentRenderers;
        _dispatcher = dispatcher;
    }

    public Key<TerminalCommand> GitTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();

    public void StatusEnqueue()
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
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
                    HACK_ArgumentsString = gitStatusDashUCommand,
                    Tag = GitCliOutputParser.TagConstants.StatusEnqueue,
				};

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: _gitCliOutputParser);

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                generalTerminal.EnqueueCommand(terminalCommand);
            });
    }

    public void RefreshEnqueue(GitRepo repoAtTimeOfRequest)
    {
		StatusEnqueue();
        BranchGetAllEnqueue(repoAtTimeOfRequest);
        GetActiveBranchNameEnqueue(repoAtTimeOfRequest);
        GetOriginNameEnqueue(repoAtTimeOfRequest);
    }

    public void GetActiveBranchNameEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
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
                    HACK_ArgumentsString = terminalCommandArgs,
                    Tag = GitCliOutputParser.TagConstants.GetActiveBranchNameEnqueue
				};

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: _gitCliOutputParser);

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                generalTerminal.EnqueueCommand(terminalCommand);
            });
    }

    public void GetOriginNameEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
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
                    HACK_ArgumentsString = terminalCommandArgs,
                    Tag = GitCliOutputParser.TagConstants.GetOriginNameEnqueue,
				};

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: _gitCliOutputParser);

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                generalTerminal.EnqueueCommand(terminalCommand);
            });
    }

    public void AddEnqueue(GitRepo repoAtTimeOfRequest)
    {
		_backgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git add",
            async () =>
			{
				var localGitState = _gitStateWrap.Value;

		        if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
		            return;
		
		        var filesBuilder =  new StringBuilder();
		
		        foreach (var gitFile in localGitState.SelectedFileList)
		        {
		            var relativePathString = gitFile.RelativePathString;
		
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
                    ContinueWith: () =>
					{
						StatusEnqueue();
						return Task.CompletedTask;
					});
		
		        var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
		        generalTerminal.EnqueueCommand(terminalCommand);
			});
    }
	
	public void UnstageEnqueue(GitRepo repoAtTimeOfRequest)
    {
		_backgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git unstage",
            async () =>
			{
				var localGitState = _gitStateWrap.Value;

		        if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
		            return;
		
		        var filesBuilder =  new StringBuilder();
		
		        foreach (var gitFile in localGitState.SelectedFileList)
		        {
		            var relativePathString = gitFile.RelativePathString;
		
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
                    ContinueWith: () =>
					{
						StatusEnqueue();
						return Task.CompletedTask;
					});
		
		        var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
		        generalTerminal.EnqueueCommand(terminalCommand);
			});
    }
	
	public void CommitEnqueue(GitRepo repoAtTimeOfRequest, string commitSummary)
    {
		_backgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
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
                    ContinueWith: () =>
					{
						StatusEnqueue();

						NotificationHelper.DispatchInformative(
							"Git: committed",
                            commitSummary,
							_commonComponentRenderers,
							_dispatcher,
							TimeSpan.FromSeconds(5));

						return Task.CompletedTask;
                    });

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                generalTerminal.EnqueueCommand(gitCommitCommand);
            });
    }

    public void BranchNewEnqueue(GitRepo repoAtTimeOfRequest, string branchName)
    {
        if (string.IsNullOrWhiteSpace(branchName))
            NotificationHelper.DispatchError(nameof(BranchNewEnqueue), "branchName was null or whitespace", _commonComponentRenderers, _dispatcher, TimeSpan.FromSeconds(6));

        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
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
                    ContinueWith: () =>
					{
						RefreshEnqueue(repoAtTimeOfRequest);
						return Task.CompletedTask;
					});

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                generalTerminal.EnqueueCommand(terminalCommand);
            });
    }

    public void BranchGetAllEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
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
                    HACK_ArgumentsString = argumentsString,
                    Tag = GitCliOutputParser.TagConstants.BranchGetAllEnqueue,
				};

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: _gitCliOutputParser);

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                generalTerminal.EnqueueCommand(terminalCommand);
            });
    }
    
    public void BranchSetEnqueue(GitRepo repoAtTimeOfRequest, string branchName)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
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
                    HACK_ArgumentsString = argumentsString,
                    Tag = GitCliOutputParser.TagConstants.BranchSetEnqueue,
				};

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: _gitCliOutputParser,
                    ContinueWith: () =>
					{
						RefreshEnqueue(repoAtTimeOfRequest);
						return Task.CompletedTask;
					});

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                generalTerminal.EnqueueCommand(terminalCommand);
            });
    }
    
    public void PushToOriginWithTrackingEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
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
                    HACK_ArgumentsString = argumentsString,
                    Tag = GitCliOutputParser.TagConstants.PushToOriginWithTrackingEnqueue
				};

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: _gitCliOutputParser,
                    ContinueWith: () =>
					{
						RefreshEnqueue(repoAtTimeOfRequest);
						return Task.CompletedTask;
					});

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                generalTerminal.EnqueueCommand(terminalCommand);
            });
    }

    public void PullEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
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
                    HACK_ArgumentsString = argumentsString,
                    Tag = GitCliOutputParser.TagConstants.PullEnqueue
				};

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: _gitCliOutputParser,
                    ContinueWith: () =>
					{
						RefreshEnqueue(repoAtTimeOfRequest);
						return Task.CompletedTask;
					});

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                generalTerminal.EnqueueCommand(terminalCommand);
            });
    }
    
    public void FetchEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
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
                    HACK_ArgumentsString = argumentsString,
                    Tag = GitCliOutputParser.TagConstants.FetchEnqueue
				};

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: _gitCliOutputParser,
                    ContinueWith: () =>
					{
						RefreshEnqueue(repoAtTimeOfRequest);
						return Task.CompletedTask;
					});

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                generalTerminal.EnqueueCommand(terminalCommand);
            });
    }
    
    public void LogFileEnqueue(
        GitRepo repoAtTimeOfRequest,
        string relativePathToFile,
        Func<GitCliOutputParser, Task> callback)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git log file",
            async () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
                    return;

                var terminalCommandArgs = $"log -p {relativePathToFile}";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { terminalCommandArgs })
                {
                    HACK_ArgumentsString = terminalCommandArgs,
                    Tag = GitCliOutputParser.TagConstants.LogFileEnqueue
				};

                var terminalCommand = new TerminalCommand(
                    GitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    OutputParser: _gitCliOutputParser,
                    ContinueWith: () =>
                    {
                        return callback.Invoke(_gitCliOutputParser);
                    });

                var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
                generalTerminal.EnqueueCommand(terminalCommand);
            });
    }
}
