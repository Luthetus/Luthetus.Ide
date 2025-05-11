using System.Text;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Ide.RazorLib.Terminals.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.Git.BackgroundTasks.Models;
using Luthetus.Extensions.Git.CommandLines.Models;
using Luthetus.Extensions.Git.ComponentRenderers.Models;

namespace Luthetus.Extensions.Git.Models;

public class GitIdeApi : IBackgroundTaskGroup
{
	private readonly GitTreeViews _gitTreeViews;
    private readonly IIdeComponentRenderers _ideComponentRenderers;
    private readonly ITreeViewService _treeViewService;
    private readonly GitBackgroundTaskApi _gitBackgroundTaskApi;
    private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
    private readonly ITerminalService _terminalService;
	private readonly GitCliOutputParser _gitCliOutputParser;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly BackgroundTaskService _backgroundTaskService;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly INotificationService _notificationService;
    
    private readonly Throttle _throttle = new(TimeSpan.FromMilliseconds(300));

    public GitIdeApi(
    	GitTreeViews gitTreeViews,
        IIdeComponentRenderers ideComponentRenderers,
        ITreeViewService treeViewService,
    	GitBackgroundTaskApi gitBackgroundTaskApi,
		IdeBackgroundTaskApi ideBackgroundTaskApi,
        ITerminalService terminalService,
		GitCliOutputParser gitCliOutputParser,
		IEnvironmentProvider environmentProvider,
		BackgroundTaskService backgroundTaskService,
        ICommonComponentRenderers commonComponentRenderers,
        INotificationService notificationService)
    {
    	_gitTreeViews = gitTreeViews;
        _ideComponentRenderers = ideComponentRenderers;
        _treeViewService = treeViewService;
    	_gitBackgroundTaskApi = gitBackgroundTaskApi;
		_ideBackgroundTaskApi = ideBackgroundTaskApi;
        _terminalService = terminalService;
		_gitCliOutputParser = gitCliOutputParser;
		_environmentProvider = environmentProvider;
		_backgroundTaskService = backgroundTaskService;
        _commonComponentRenderers = commonComponentRenderers;
        _notificationService = notificationService;
    }

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public string Name { get; } = nameof(GitIdeApi);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<GitIdeApiWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();

    private readonly Queue<GitRepo> _queue_general_repoAtTimeOfRequest = new();

    public Key<TerminalCommandRequest> GitTerminalCommandRequestKey { get; } = Key<TerminalCommandRequest>.NewKey();

    public void Enqueue_Status()
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitIdeApiWorkKind.Status);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_Status()
    {
        var localGitState = GetGitState();

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
                	
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
		return ValueTask.CompletedTask;
    }

    public void RefreshEnqueue(GitRepo repoAtTimeOfRequest)
    {
		Enqueue_Status();
        Enqueue_BranchGetAll(repoAtTimeOfRequest);
        Enqueue_GetActiveBranchName(repoAtTimeOfRequest);
        Enqueue_GetOriginName(repoAtTimeOfRequest);
    }

    public void Enqueue_GetActiveBranchName(GitRepo repoAtTimeOfRequest)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitIdeApiWorkKind.GetActiveBranchName);
            _queue_general_repoAtTimeOfRequest.Enqueue(repoAtTimeOfRequest);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_GetActiveBranchName(GitRepo repoAtTimeOfRequest)
    {
        var localGitState = GetGitState();

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
                	
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
		return ValueTask.CompletedTask;
    }

    public void Enqueue_GetOriginName(GitRepo repoAtTimeOfRequest)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitIdeApiWorkKind.GetOriginName);
            _queue_general_repoAtTimeOfRequest.Enqueue(repoAtTimeOfRequest);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_GetOriginName(GitRepo repoAtTimeOfRequest)
    {
        var localGitState = GetGitState();

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
                	
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
		return ValueTask.CompletedTask;
    }

    public void Enqueue_Add(GitRepo repoAtTimeOfRequest)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitIdeApiWorkKind.Add);
            _queue_general_repoAtTimeOfRequest.Enqueue(repoAtTimeOfRequest);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_Add(GitRepo repoAtTimeOfRequest)
    {
		var localGitState = GetGitState();

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
                Enqueue_Status();
				return Task.CompletedTask;
            }
        };
                	
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
		return ValueTask.CompletedTask;
    }
	
	public void Enqueue_Unstage(GitRepo repoAtTimeOfRequest)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitIdeApiWorkKind.Unstage);
            _queue_general_repoAtTimeOfRequest.Enqueue(repoAtTimeOfRequest);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_Unstage(GitRepo repoAtTimeOfRequest)
    {
		var localGitState = GetGitState();

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
                Enqueue_Status();
                return Task.CompletedTask;
            }
        };
                	
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
		return ValueTask.CompletedTask;
    }

    private readonly Queue<(GitRepo repoAtTimeOfRequest, string commitSummary)> _queue_Commit = new();

	public void Enqueue_Commit(GitRepo repoAtTimeOfRequest, string commitSummary)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitIdeApiWorkKind.Commit);
            _queue_Commit.Enqueue((repoAtTimeOfRequest, commitSummary));
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_Commit(GitRepo repoAtTimeOfRequest, string commitSummary)
    {
		var localGitState = GetGitState();

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
                Enqueue_Status();
	
				NotificationHelper.DispatchInformative(
					"Git: committed",
	                commitSummary,
					_commonComponentRenderers,
					_notificationService,
					TimeSpan.FromSeconds(5));
	
				return Task.CompletedTask;
			}
        };
                	
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
		return ValueTask.CompletedTask;
    }

    private readonly Queue<(GitRepo repoAtTimeOfRequest, string branchName)> _queue_BranchNew = new();

    public void Enqueue_BranchNew(GitRepo repoAtTimeOfRequest, string branchName)
    {
        if (string.IsNullOrWhiteSpace(branchName))
            NotificationHelper.DispatchError(nameof(Enqueue_BranchNew), "branchName was null or whitespace", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(6));

        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitIdeApiWorkKind.BranchNew);
            _queue_BranchNew.Enqueue((repoAtTimeOfRequest, branchName));
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_BranchNew(GitRepo repoAtTimeOfRequest, string branchName)
    {
        if (string.IsNullOrWhiteSpace(branchName))
            NotificationHelper.DispatchError(nameof(Do_BranchNew), "branchName was null or whitespace", _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(6));

        var localGitState = GetGitState();

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
                	
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
		return ValueTask.CompletedTask;
    }

    public void Enqueue_BranchGetAll(GitRepo repoAtTimeOfRequest)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitIdeApiWorkKind.BranchGetAll);
            _queue_general_repoAtTimeOfRequest.Enqueue(repoAtTimeOfRequest);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_BranchGetAll(GitRepo repoAtTimeOfRequest)
    {
        var localGitState = GetGitState();

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
                	
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
		return ValueTask.CompletedTask;
    }

    private readonly Queue<(GitRepo repoAtTimeOfRequest, string branchName)> _queue_BranchSet = new();

    public void Enqueue_BranchSet(GitRepo repoAtTimeOfRequest, string branchName)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitIdeApiWorkKind.BranchSet);
            _queue_BranchSet.Enqueue((repoAtTimeOfRequest, branchName));
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_BranchSet(GitRepo repoAtTimeOfRequest, string branchName)
    {
        var localGitState = GetGitState();

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
                	
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
		return ValueTask.CompletedTask;
    }
    
    public void Enqueue_PushToOriginWithTracking(GitRepo repoAtTimeOfRequest)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitIdeApiWorkKind.PushToOriginWithTracking);
            _queue_general_repoAtTimeOfRequest.Enqueue(repoAtTimeOfRequest);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_PushToOriginWithTracking(GitRepo repoAtTimeOfRequest)
    {
        var localGitState = GetGitState();

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
                	
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
		return ValueTask.CompletedTask;
    }

    public void Enqueue_Pull(GitRepo repoAtTimeOfRequest)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitIdeApiWorkKind.Pull);
            _queue_general_repoAtTimeOfRequest.Enqueue(repoAtTimeOfRequest);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_Pull(GitRepo repoAtTimeOfRequest)
    {
        var localGitState = GetGitState();

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
                	
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
		return ValueTask.CompletedTask;
    }
    
    public void Enqueue_Fetch(GitRepo repoAtTimeOfRequest)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitIdeApiWorkKind.Fetch);
            _queue_general_repoAtTimeOfRequest.Enqueue(repoAtTimeOfRequest);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_Fetch(GitRepo repoAtTimeOfRequest)
    {
        var localGitState = GetGitState();

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
                	
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
		return ValueTask.CompletedTask;
    }

    private readonly
        Queue<(GitRepo repoAtTimeOfRequest, string relativePathToFile, Func<GitCliOutputParser, string, Task> callback)>
        _queue_general_callback = new();

    public void Enqueue_LogFile(
        GitRepo repoAtTimeOfRequest,
        string relativePathToFile,
        Func<GitCliOutputParser, string, Task> callback)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitIdeApiWorkKind.LogFile);
            _queue_general_callback.Enqueue((repoAtTimeOfRequest, relativePathToFile, callback));
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_LogFile(
        GitRepo repoAtTimeOfRequest,
        string relativePathToFile,
        Func<GitCliOutputParser, string, Task> callback)
    {
        var localGitState = GetGitState();

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
                	
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
		return ValueTask.CompletedTask;
    }

    public void Enqueue_ShowFile(
        GitRepo repoAtTimeOfRequest,
        string relativePathToFile,
        Func<GitCliOutputParser, string, Task> callback)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitIdeApiWorkKind.ShowFile);
            _queue_general_callback.Enqueue((repoAtTimeOfRequest, relativePathToFile, callback));
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_ShowFile(
        GitRepo repoAtTimeOfRequest,
        string relativePathToFile,
        Func<GitCliOutputParser, string, Task> callback)
    {
        var localGitState = GetGitState();

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
                		
                _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(showTerminalCommandRequest);
                return Task.CompletedTask;
            }
        };
                	
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(logTerminalCommandRequest);
		return ValueTask.CompletedTask;
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

    private readonly
        Queue<(GitRepo repoAtTimeOfRequest, string relativePathToFile, Func<GitCliOutputParser, string, List<int>, Task> callback)>
        _queue_DiffFile = new();

    public void Enqueue_DiffFile(
        GitRepo repoAtTimeOfRequest,
        string relativePathToFile,
        Func<GitCliOutputParser, string, List<int>, Task> callback)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(GitIdeApiWorkKind.DiffFile);
            _queue_DiffFile.Enqueue((repoAtTimeOfRequest, relativePathToFile, callback));
            _backgroundTaskService.EnqueueGroup(this);
        }
    }
    
    public ValueTask Do_DiffFile(
        GitRepo repoAtTimeOfRequest,
        string relativePathToFile,
        Func<GitCliOutputParser, string, List<int>, Task> callback)
    {
        var localGitState = GetGitState();

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
                	
        _terminalService.GetTerminalState().TerminalMap[TerminalFacts.GENERAL_KEY].EnqueueCommand(terminalCommandRequest);
		return ValueTask.CompletedTask;
    }
	
	private GitState _gitState = new();
	
	public event Action? GitStateChanged;
	
	public GitState GetGitState() => _gitState;
    
    /// <summary>
    /// If the expected path is not the actual path, then the git file list will NOT be changed.
    /// </summary>
    public void ReduceSetStatusAction(
        GitRepo repo,
        List<GitFile> untrackedFileList,
        List<GitFile> stagedFileList,
        List<GitFile> unstagedFileList,
        int? behindByCommitCount,
        int? aheadByCommitCount)
    {
    	var inState = GetGitState();
    
        if (inState.Repo != repo)
        {
            // Git folder was changed while the text was being parsed,
            // throw away the result since it is thereby invalid.
            GitStateChanged?.Invoke();
            return;
        }

        _gitState = inState with
        {
            UntrackedFileList = untrackedFileList,
            StagedFileList = stagedFileList,
            UnstagedFileList = unstagedFileList,
            BehindByCommitCount = behindByCommitCount,
            AheadByCommitCount = aheadByCommitCount,
        };
        
        GitStateChanged?.Invoke();
        return;
    }

    public void ReduceSetGitOriginAction(GitRepo repo, string origin)
    {
    	var inState = GetGitState();
    
        if (inState.Repo != repo)
        {
            // Git folder was changed while the text was being parsed,
            // throw away the result since it is thereby invalid.
            GitStateChanged?.Invoke();
        	return;
        }

        _gitState = inState with
        {
            Origin = origin
        };
        
        GitStateChanged?.Invoke();
        return;
    }
    
    public void ReduceSetBranchAction(GitRepo repo, string branch)
    {
    	var inState = GetGitState();
    
        if (inState.Repo != repo)
        {
            // Git folder was changed while the text was being parsed,
            // throw away the result since it is thereby invalid.
            GitStateChanged?.Invoke();
        	return;
        }

        _gitState = inState with
        {
            Branch = branch
        };
        
        GitStateChanged?.Invoke();
        return;
    }
    
    public void ReduceSetBranchListAction(GitRepo repo, List<string> branchList)
    {
    	var inState = GetGitState();
    
        if (inState.Repo != repo)
        {
            // Git folder was changed while the text was being parsed,
            // throw away the result since it is thereby invalid.
            GitStateChanged?.Invoke();
        	return;
        }

        _gitState = inState with
        {
            BranchList = branchList
        };
        
        GitStateChanged?.Invoke();
        return;
    }
    
    public void ReduceSetGitFolderAction(GitRepo? repo)
    {
    	var inState = GetGitState();
    
        _gitState = inState with
        {
            Repo = repo,
            UntrackedFileList = new List<GitFile>(),
            StagedFileList = new List<GitFile>(),
            UnstagedFileList = new List<GitFile>(),
            SelectedFileList = new List<GitFile>(),
            ActiveTasks = new List<GitTask>(),
            Branch = null,
            Origin = null,
            AheadByCommitCount = null,
            BehindByCommitCount = null,
            BranchList = new List<string>(),
            Upstream = null,
        };
        
        GitStateChanged?.Invoke();
        return;
    }

    public void ReduceSetGitStateWithAction(Func<GitState, GitState> withFunc)
    {
    	var inState = GetGitState();
    
        _gitState = withFunc.Invoke(inState);
        
        GitStateChanged?.Invoke();
        return;
    }

	public Task HandleSetFileListAction()
	{
		_throttle.Run(_ =>
        {
            var gitState = GetGitState();

            var untrackedTreeViewList = gitState.UntrackedFileList.Select(x => new TreeViewGitFile(
                    x,
                    _gitTreeViews,
                    false,
                    false))
                .ToArray();

            var untrackedFileGroupTreeView = new TreeViewGitFileGroup(
                "Untracked",
                _ideComponentRenderers,
                _commonComponentRenderers,
                true,
                true);

            untrackedFileGroupTreeView.SetChildList(untrackedTreeViewList);

            var stagedTreeViewList = gitState.StagedFileList.Select(x => new TreeViewGitFile(
                    x,
                    _gitTreeViews,
                    false,
                    false))
                .ToArray();

            var stagedFileGroupTreeView = new TreeViewGitFileGroup(
                "Staged",
                _ideComponentRenderers,
                _commonComponentRenderers,
                true,
                true);

            stagedFileGroupTreeView.SetChildList(stagedTreeViewList);
            
            var unstagedTreeViewList = gitState.UnstagedFileList.Select(x => new TreeViewGitFile(
                    x,
                    _gitTreeViews,
                    false,
                    false))
                .ToArray();

            var unstagedFileGroupTreeView = new TreeViewGitFileGroup(
                "Not-staged",
                _ideComponentRenderers,
                _commonComponentRenderers,
                true,
                true);

            unstagedFileGroupTreeView.SetChildList(unstagedTreeViewList);

            var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(
                stagedFileGroupTreeView,
                unstagedFileGroupTreeView,
                untrackedFileGroupTreeView);

            var firstNode = untrackedTreeViewList.FirstOrDefault();

            IReadOnlyList<TreeViewNoType> activeNodes = firstNode is null
                ? Array.Empty<TreeViewNoType>()
                : new List<TreeViewNoType> { firstNode };

            if (!_treeViewService.TryGetTreeViewContainer(GitState.TreeViewGitChangesKey, out var container))
            {
                _treeViewService.ReduceRegisterContainerAction(new TreeViewContainer(
                    GitState.TreeViewGitChangesKey,
                    adhocRoot,
                    activeNodes));
            }
            else
            {
                _treeViewService.ReduceWithRootNodeAction(GitState.TreeViewGitChangesKey, adhocRoot);

                _treeViewService.ReduceSetActiveNodeAction(
                    GitState.TreeViewGitChangesKey,
                    firstNode,
                    true,
                    false);
            }

            return Task.CompletedTask;
        });
        
        return Task.CompletedTask;
    }
    
	public Task HandleSetRepoAction(GitRepo? repo)
	{
        if (repo is not null)
            _gitBackgroundTaskApi.Git.RefreshEnqueue(repo);

        return Task.CompletedTask;
    }

    public IBackgroundTaskGroup? EarlyBatchOrDefault(IBackgroundTaskGroup oldEvent)
    {
        return null;
    }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        GitIdeApiWorkKind workKind;

        lock (_workLock)
        {
            if (!_workKindQueue.TryDequeue(out workKind))
                return ValueTask.CompletedTask;
        }

        switch (workKind)
        {
            case GitIdeApiWorkKind.Status:
            {
                return Do_Status();
            }
            case GitIdeApiWorkKind.GetActiveBranchName:
            {
                var args = _queue_general_repoAtTimeOfRequest.Dequeue();
                return Do_GetActiveBranchName(args);
            }
            case GitIdeApiWorkKind.GetOriginName:
            {
                var args = _queue_general_repoAtTimeOfRequest.Dequeue();
                return Do_GetOriginName(args);
            }
            case GitIdeApiWorkKind.Add:
            {
                var args = _queue_general_repoAtTimeOfRequest.Dequeue();
                return Do_Add(args);
            }
            case GitIdeApiWorkKind.Unstage:
            {
                var args = _queue_general_repoAtTimeOfRequest.Dequeue();
                return Do_Unstage(args);
            }
            case GitIdeApiWorkKind.Commit:
            {
                var args = _queue_Commit.Dequeue();
                return Do_Commit(args.repoAtTimeOfRequest, args.commitSummary);
            }
            case GitIdeApiWorkKind.BranchNew:
            {
                var args = _queue_BranchNew.Dequeue();
                return Do_BranchNew(args.repoAtTimeOfRequest, args.branchName);
            }
            case GitIdeApiWorkKind.BranchGetAll:
            {
                var args = _queue_general_repoAtTimeOfRequest.Dequeue();
                return Do_BranchGetAll(args);
            }
            case GitIdeApiWorkKind.BranchSet:
            {
                var args = _queue_BranchSet.Dequeue();
                return Do_BranchSet(args.repoAtTimeOfRequest, args.branchName);
            }
            case GitIdeApiWorkKind.PushToOriginWithTracking:
            {
                var args = _queue_general_repoAtTimeOfRequest.Dequeue();
                return Do_PushToOriginWithTracking(args);
            }
            case GitIdeApiWorkKind.Pull:
            {
                var args = _queue_general_repoAtTimeOfRequest.Dequeue();
                return Do_Pull(args);
            }
            case GitIdeApiWorkKind.Fetch:
            {
                var args = _queue_general_repoAtTimeOfRequest.Dequeue();
                return Do_Fetch(args);
            }
            case GitIdeApiWorkKind.LogFile:
            {
                var args = _queue_general_callback.Dequeue();
                return Do_LogFile(args.repoAtTimeOfRequest, args.relativePathToFile, args.callback);
            }
            case GitIdeApiWorkKind.ShowFile:
            {
                var args = _queue_general_callback.Dequeue();
                return Do_ShowFile(args.repoAtTimeOfRequest, args.relativePathToFile, args.callback);
            }
            case GitIdeApiWorkKind.DiffFile:
            {
                var args = _queue_DiffFile.Dequeue();
                return Do_DiffFile(args.repoAtTimeOfRequest, args.relativePathToFile, args.callback);
            }
            default:
            {
                Console.WriteLine($"{nameof(GitIdeApi)} {nameof(HandleEvent)} default case");
                return ValueTask.CompletedTask;
            }
        }
    }
}
