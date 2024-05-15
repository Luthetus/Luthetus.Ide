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

	public static Key<TerminalCommand> GitStatusTerminalCommandKey { get; } = new(Guid.Parse("fde9dba4-0219-4a77-9c8d-0ff2b4f9109e"));
    public Key<TerminalCommand> GitAddTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();
    public Key<TerminalCommand> GitUnstageTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();
    public Key<TerminalCommand> GitCommitTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();

    public async Task GitStatusExecute()
    {
		await _backgroundTaskService.EnqueueAsync(
			Key<BackgroundTask>.NewKey(),
			ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git status",
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
		
		        var gitStatusCommand = new TerminalCommand(
		            GitStatusTerminalCommandKey,
		            formattedCommand,
		            localGitState.Repo.AbsolutePath.Value,
		            OutputParser: gitCliOutputParser);
		
		        var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
		        await generalTerminal
		            .EnqueueCommandAsync(gitStatusCommand)
		            .ConfigureAwait(false);
			});
    }

	public async Task GitAddExecute(GitRepo repoAtTimeOfRequest)
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
		
		        var argumentsString = "add " + filesBuilder.ToString();
		
		        var formattedCommand = new FormattedCommand(
		            GitCliFacts.TARGET_FILE_NAME,
		            new string[] { argumentsString })
		        {
		            HACK_ArgumentsString = argumentsString
		        };
		        
		        var gitAddCommand = new TerminalCommand(
		            GitAddTerminalCommandKey,
		            formattedCommand,
		            localGitState.Repo.AbsolutePath.Value,
                    ContinueWith: GitStatusExecute);
		
		        var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
		        await generalTerminal
		            .EnqueueCommandAsync(gitAddCommand)
		            .ConfigureAwait(false);
			});
    }
	
	public async Task GitUnstageExecute(GitRepo repoAtTimeOfRequest)
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
		
		        var argumentsString = "restore --staged " + filesBuilder.ToString();
		
		        var formattedCommand = new FormattedCommand(
		            GitCliFacts.TARGET_FILE_NAME,
		            new string[] { argumentsString })
		        {
		            HACK_ArgumentsString = argumentsString
		        };
		        
		        var gitUnstageCommand = new TerminalCommand(
                    GitUnstageTerminalCommandKey,
		            formattedCommand,
		            localGitState.Repo.AbsolutePath.Value,
                    ContinueWith: GitStatusExecute);
		
		        var generalTerminal = _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_TERMINAL_KEY];
		        await generalTerminal
		            .EnqueueCommandAsync(gitUnstageCommand)
		            .ConfigureAwait(false);
			});
    }
	
	public async Task GitCommitExecute(GitRepo repoAtTimeOfRequest, string commitSummary)
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
                    GitCommitTerminalCommandKey,
                    formattedCommand,
                    localGitState.Repo.AbsolutePath.Value,
                    ContinueWith: async () =>
					{
						await GitStatusExecute();

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
}
