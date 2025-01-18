using System.Text;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Extensions.Git.States;
using Luthetus.Extensions.Git.BackgroundTasks.Models;
using Luthetus.Extensions.Git.CommandLines.Models;

namespace Luthetus.Extensions.Git.Models;

public class GitIdeApi
{
    private readonly GitBackgroundTaskApi _gitBackgroundTaskApi;
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly IState<TerminalState> _terminalStateWrap;
    private readonly IState<GitState> _gitStateWrap;
	private readonly GitCliOutputParser _gitCliOutputParser;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly IDispatcher _dispatcher;

    public GitIdeApi(
    	GitBackgroundTaskApi gitBackgroundTaskApi,
		IdeBackgroundTaskApi ideBackgroundTaskApi,
        IState<TerminalState> terminalStateWrap,
        IState<GitState> gitStateWrap,
		GitCliOutputParser gitCliOutputParser,
		IEnvironmentProvider environmentProvider,
		IBackgroundTaskService backgroundTaskService,
        ICommonComponentRenderers commonComponentRenderers,
        IDispatcher dispatcher)
    {
    	_gitBackgroundTaskApi = gitBackgroundTaskApi;
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
        _terminalStateWrap = terminalStateWrap;
		_gitStateWrap = gitStateWrap;
		_gitCliOutputParser = gitCliOutputParser;
		_environmentProvider = environmentProvider;
		_backgroundTaskService = backgroundTaskService;
        _commonComponentRenderers = commonComponentRenderers;
        _dispatcher = dispatcher;
    }

    public Key<TerminalCommandRequest> GitTerminalCommandRequestKey { get; } = Key<TerminalCommandRequest>.NewKey();

    public void StatusEnqueue()
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git status -u",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null)
					return ValueTask.CompletedTask;

                var gitStatusDashUCommand = $"{GitCliFacts.STATUS_COMMAND} -u";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { gitStatusDashUCommand })
                {
                    HACK_ArgumentsString = gitStatusDashUCommand,
                    Tag = GitCliOutputParser.TagConstants.StatusEnqueue,
				};

                var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		var textSpanList = _gitCliOutputParser.StatusParseEntire(
                			parsedCommand.OutputCache.ToString());
                			
                		_gitCliOutputParser.DispatchSetStatusAction();
                			
                		parsedCommand.TextSpanList = textSpanList;
                		return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
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
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var terminalCommandArgs = $"branch --show-current";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { terminalCommandArgs })
                {
                    HACK_ArgumentsString = terminalCommandArgs,
                    Tag = GitCliOutputParser.TagConstants.GetActiveBranchNameEnqueue
				};

                var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		_gitCliOutputParser.GetBranchParse(
                			parsedCommand.OutputCache.ToString());
                			
                		_gitCliOutputParser.DispatchSetBranchAction();
                		return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }

    public void GetOriginNameEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git get origin name",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var terminalCommandArgs = $"config --get remote.origin.url";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { terminalCommandArgs })
                {
                    HACK_ArgumentsString = terminalCommandArgs,
                    Tag = GitCliOutputParser.TagConstants.GetOriginNameEnqueue,
				};
                    
                var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		_gitCliOutputParser.GetOriginParse(
                			parsedCommand.OutputCache.ToString());
                		
                		_gitCliOutputParser.DispatchSetOriginAction();
                		return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
		});
    }

    public void AddEnqueue(GitRepo repoAtTimeOfRequest)
    {
		_backgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git add",
            () =>
			{
				var localGitState = _gitStateWrap.Value;

		        if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

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
					
				var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
            	{
            		ContinueWithFunc = parsedCommand =>
            		{
            			StatusEnqueue();
						return Task.CompletedTask;
            		}
            	};
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
	
	public void UnstageEnqueue(GitRepo repoAtTimeOfRequest)
    {
		_backgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git unstage",
            () =>
			{
				var localGitState = _gitStateWrap.Value;

		        if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

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
		        
				var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		StatusEnqueue();
                		return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
	
	public void CommitEnqueue(GitRepo repoAtTimeOfRequest, string commitSummary)
    {
		_backgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git commit",
            () =>
			{
				var localGitState = _gitStateWrap.Value;

		        if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var argumentsString = $"commit -m \"{commitSummary}\"";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString
                };

                var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
	                	StatusEnqueue();
	
						NotificationHelper.DispatchInformative(
							"Git: committed",
	                        commitSummary,
							_commonComponentRenderers,
							_dispatcher,
							TimeSpan.FromSeconds(5));
	
						return Task.CompletedTask;
					}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
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
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var argumentsString = "checkout -b " + branchName;

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString
                };
                
				var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		RefreshEnqueue(repoAtTimeOfRequest);
						return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }

    public void BranchGetAllEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git branch -a",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var argumentsString = "branch -a";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString,
                    Tag = GitCliOutputParser.TagConstants.BranchGetAllEnqueue,
				};
                    
                var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		_gitCliOutputParser.GetBranchListEntire(
                			parsedCommand.OutputCache.ToString());
                			
                		_gitCliOutputParser.DispatchSetBranchListAction();
                		return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
    
    public void BranchSetEnqueue(GitRepo repoAtTimeOfRequest, string branchName)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            $"git checkout {branchName}",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
                    return ValueTask.CompletedTask;

                var argumentsString = $"checkout {branchName}";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString,
                    Tag = GitCliOutputParser.TagConstants.BranchSetEnqueue,
				};

				var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		RefreshEnqueue(repoAtTimeOfRequest);
                		return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
    
    public void PushToOriginWithTrackingEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git push -u origin {branchName will go here}",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

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

				var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
	                	RefreshEnqueue(repoAtTimeOfRequest);
						return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }

    public void PullEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git pull",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var argumentsString = $"pull";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString,
                    Tag = GitCliOutputParser.TagConstants.PullEnqueue
				};

				var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		RefreshEnqueue(repoAtTimeOfRequest);
						return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
    
    public void FetchEnqueue(GitRepo repoAtTimeOfRequest)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git fetch",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var argumentsString = $"fetch";

                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { argumentsString })
                {
                    HACK_ArgumentsString = argumentsString,
                    Tag = GitCliOutputParser.TagConstants.FetchEnqueue
				};

			    var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		RefreshEnqueue(repoAtTimeOfRequest);
						return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
    
    public void LogFileEnqueue(
        GitRepo repoAtTimeOfRequest,
        string relativePathToFile,
        Func<GitCliOutputParser, string, Task> callback)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git log file",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var terminalCommandArgs = $"log -p {relativePathToFile}";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { terminalCommandArgs })
                {
                    HACK_ArgumentsString = terminalCommandArgs,
                    Tag = GitCliOutputParser.TagConstants.LogFileEnqueue
				};

                var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		return callback.Invoke(
                			_gitCliOutputParser,
                			parsedCommand.OutputCache.ToString());
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
    
    public void ShowFileEnqueue(
        GitRepo repoAtTimeOfRequest,
        string relativePathToFile,
        Func<GitCliOutputParser, string, Task> callback)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git show file",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				// Example output:
				/*
				PS C:\Users\hunte\Repos\Demos\BlazorApp4NetCoreDbg> git log BlazorApp4NetCoreDbg/Pages/FetchData.razor
				commit 87c2893b1006defc36770c166ab13fdbc6b7f959
				Author: Luthetus <45454132+huntercfreeman@users.noreply.github.com>
				Date:   Fri May 3 16:15:17 2024 -0400
				
				    Abc123 3
				*/
				
				var skipTextLength = "commit ".Length;
				var takeTextLength = "87c2893b1006defc36770c166ab13fdbc6b7f959".Length;
				
				var logTerminalCommandArgs = $"log -p {relativePathToFile}";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { logTerminalCommandArgs })
                {
                    HACK_ArgumentsString = logTerminalCommandArgs,
                    Tag = GitCliOutputParser.TagConstants.LogFileEnqueue
				};

                var logTerminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = gitHashParsedCommand =>
                	{
                		var hash = gitHashParsedCommand.OutputCache
                			.ToString()
                			.Substring(skipTextLength, takeTextLength)
                			.Trim();
                	
                		var showTerminalCommandRequest = new TerminalCommandRequest(
                			$"git show {hash}:{relativePathToFile}",
                			localGitState.Repo.AbsolutePath.Value)
                		{
                			ContinueWithFunc = parsedCommand =>
                			{
                				var output = parsedCommand.OutputCache.ToString();
                			
                				if (output.StartsWith("ï»¿"))
                					output = output["ï»¿".Length..];
                			
                				return callback.Invoke(
		                			_gitCliOutputParser,
		                			output);
                			}
                		};
                		
                		_terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(showTerminalCommandRequest);
                		return Task.CompletedTask;
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(logTerminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
    
    private List<int> GetPlusMarkedLineIndexList(string gitLogDashPOutput)
	{
		var stringWalker = new StringWalker(new ResourceUri("/getPlusMarkedLineIndexList.txt"), gitLogDashPOutput);
		var linesReadCount = 0;
		
		// The hunk header looks like:
		// "@@ -1,6 +1,23 @@"
		var atAtReadCount = 0;
		var oldAndNewHaveEqualFirstLine = false;
		int? sourceLineNumber = null;
		
		var isFirstCharacterOnLine = false;
		
		var plusMarkedLineIndexList = new List<int>();
		
		while (!stringWalker.IsEof)
		{
			if (WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(stringWalker.CurrentCharacter))
			{
				if (stringWalker.CurrentCharacter == '\r' && stringWalker.NextCharacter == '\n')
					_ = stringWalker.ReadCharacter();
					
				linesReadCount++;
				isFirstCharacterOnLine = true;
				
				if (sourceLineNumber is not null)
					sourceLineNumber++;
			}
			else if (linesReadCount == 4 && sourceLineNumber is null)
			{
				// Naively going to assume that the 5th line is always the start of the hunk header... for now
				if (stringWalker.CurrentCharacter == '@' && stringWalker.NextCharacter == '@')
				{
					atAtReadCount++;
					
					if (atAtReadCount == 2)
					{
						if (WhitespaceFacts.LINE_ENDING_CHARACTER_LIST.Contains(stringWalker.PeekCharacter(2)))
						{
							// If immediately after the hunk header there is a newline character,
							// then the old and new text are NOT equal with respects to the first line.
						}
						else
						{
							// If after the hunk header there is NOT a newline character,
							// then the old and new text are equal with respects to the first line.
							//
							// (Does modificaton imply deletion of old, and insertion of new?)
							oldAndNewHaveEqualFirstLine = true;
						}
						
						sourceLineNumber = 0;
					}
				}
			}
			else if (sourceLineNumber is not null)
			{
				if (isFirstCharacterOnLine && stringWalker.CurrentCharacter == '+')
					plusMarkedLineIndexList.Add(sourceLineNumber.Value - 1);
			}
		
			_ = stringWalker.ReadCharacter();
		}
		
		return plusMarkedLineIndexList;
	}
    
    public void DiffFileEnqueue(
        GitRepo repoAtTimeOfRequest,
        string relativePathToFile,
        Func<GitCliOutputParser, string, List<int>, Task> callback)
    {
        _backgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            "git diff file",
            () =>
            {
                var localGitState = _gitStateWrap.Value;

                if (localGitState.Repo is null || localGitState.Repo != repoAtTimeOfRequest)
					return ValueTask.CompletedTask;

				var terminalCommandArgs = $"diff -p {relativePathToFile}";
                var formattedCommand = new FormattedCommand(
                    GitCliFacts.TARGET_FILE_NAME,
                    new string[] { terminalCommandArgs })
                {
                    HACK_ArgumentsString = terminalCommandArgs,
                    Tag = GitCliOutputParser.TagConstants.LogFileEnqueue
				};

                var terminalCommandRequest = new TerminalCommandRequest(
                	formattedCommand.Value,
                	localGitState.Repo.AbsolutePath.Value)
                {
                	ContinueWithFunc = parsedCommand =>
                	{
                		var plusMarkedLineIndexList = GetPlusMarkedLineIndexList(parsedCommand.OutputCache.ToString());
                	
                		return callback.Invoke(
                			_gitCliOutputParser,
                			parsedCommand.OutputCache.ToString(),
                			plusMarkedLineIndexList);
                	}
                };
                	
                _terminalStateWrap.Value.TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
				return ValueTask.CompletedTask;
			});
    }
}
