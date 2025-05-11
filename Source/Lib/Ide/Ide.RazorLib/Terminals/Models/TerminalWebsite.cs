using CliWrap.EventStream;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Ide.RazorLib.Exceptions;
using Luthetus.Ide.RazorLib.Terminals.Models.Internals;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

/// <summary>
/// This implementation of <see cref="ITerminal"/> is for the website demo.
/// </summary>
public class TerminalWebsite : ITerminal, IBackgroundTaskGroup
{
	private readonly BackgroundTaskService _backgroundTaskService;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly INotificationService _notificationService;

	public TerminalWebsite(
		string displayName,
		Func<TerminalWebsite, ITerminalInteractive> terminalInteractiveFactory,
		Func<TerminalWebsite, ITerminalInput> terminalInputFactory,
		Func<TerminalWebsite, ITerminalOutput> terminalOutputFactory,
		BackgroundTaskService backgroundTaskService,
		ICommonComponentRenderers commonComponentRenderers,
		INotificationService notificationService)
	{
		DisplayName = displayName;
		TerminalInteractive = terminalInteractiveFactory.Invoke(this);
		TerminalInput = terminalInputFactory.Invoke(this);
		TerminalOutput = terminalOutputFactory.Invoke(this);
		
		_backgroundTaskService = backgroundTaskService;
		_commonComponentRenderers = commonComponentRenderers;
		_notificationService = notificationService;
	}

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.IndefiniteQueueKey;
    public string Name { get; } = nameof(TerminalWebsite);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<TerminalWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();

    public string DisplayName { get; }
	public ITerminalInteractive TerminalInteractive { get; }
	public ITerminalInput TerminalInput { get; }
	public ITerminalOutput TerminalOutput { get; }

	private CancellationTokenSource _commandCancellationTokenSource = new();

    public Key<ITerminal> Key { get; init; } = Key<ITerminal>.NewKey();
    public TerminalCommandParsed? ActiveTerminalCommandParsed { get; private set; }

	/// <summary>NOTE: the following did not work => _process?.HasExited ?? false;</summary>
    public bool HasExecutingProcess { get; private set; }

	private readonly Queue<TerminalCommandRequest> _queue_general_TerminalCommandRequest = new();

    public void EnqueueCommand(TerminalCommandRequest terminalCommandRequest)
    {
        lock (_workLock)
        {
            _workKindQueue.Enqueue(TerminalWorkKind.Command);
            _queue_general_TerminalCommandRequest.Enqueue(terminalCommandRequest);
            _backgroundTaskService.EnqueueGroup(this);
        }
    }

    public ValueTask DoCommand(TerminalCommandRequest terminalCommandRequest)
    {
        return HandleCommand(terminalCommandRequest);
    }

    public Task EnqueueCommandAsync(TerminalCommandRequest terminalCommandRequest)
    {
		return _backgroundTaskService.EnqueueAsync(
			Key<IBackgroundTaskGroup>.NewKey(),
			BackgroundTaskFacts.IndefiniteQueueKey,
			"Enqueue Command",
			() => HandleCommand(terminalCommandRequest));
    }
    
    public void ClearEnqueue()
    {
    	EnqueueCommand(new TerminalCommandRequest("clear", null));
    }
    
    public void ClearFireAndForget()
    {
    	var localHasExecutingProcess = HasExecutingProcess;
    
    	_ = Task.Run(() =>
    	{
    		if (localHasExecutingProcess)
    		{
    			TerminalOutput.ClearOutputExceptMostRecentCommand();
    		}
    		else
    		{
    			TerminalOutput.ClearOutput();
    		}
    		
    		return Task.CompletedTask;
    	});
    }

    private async ValueTask HandleCommand(TerminalCommandRequest terminalCommandRequest)
    {
    	TerminalOutput.ClearHistoryWhenExistingOutputTooLong();
    
    	var parsedCommand = await TerminalInteractive.TryHandleCommand(terminalCommandRequest);
    	ActiveTerminalCommandParsed = parsedCommand;

		if (parsedCommand is null)
			return;
    	
		try
		{
			HasExecutingProcess = true;
		
			if (parsedCommand.SourceTerminalCommandRequest.BeginWithFunc is not null)
			{
				await parsedCommand.SourceTerminalCommandRequest.BeginWithFunc
					.Invoke(parsedCommand)
					.ConfigureAwait(false);
			}
			
			await ExecuteWebsiteCliAsync(parsedCommand, HandleOutput, _commandCancellationTokenSource.Token)
				.ConfigureAwait(false);
		}
		catch (Exception e)
		{
			// TODO: This will erroneously write 'StartedCommandEvent' out twice...
			//       ...unless a check is added to see WHEN the exception was thrown.
			TerminalOutput.WriteOutput(parsedCommand, new StartedCommandEvent(-1));
		
			TerminalOutput.WriteOutput(
				parsedCommand,
				new StandardErrorCommandEvent(parsedCommand.SourceTerminalCommandRequest.CommandText + " threw an exception" + "\n"));
		
			NotificationHelper.DispatchError("Terminal Exception", e.ToString(), _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(14));
		}
		finally
		{
			if (parsedCommand.SourceTerminalCommandRequest.ContinueWithFunc is not null)
			{
				await parsedCommand.SourceTerminalCommandRequest.ContinueWithFunc
					.Invoke(parsedCommand)
					.ConfigureAwait(false);
			}
		
			HasExecutingProcess = false;
		}
	}
	
	private void HandleOutput(CommandEvent commandEvent)
	{
		TerminalOutput.WriteOutput(ActiveTerminalCommandParsed, commandEvent);
	}
	
	private Task ExecuteWebsiteCliAsync(
		TerminalCommandParsed parsedCommand,
		Action<CommandEvent> handleOutputAction,
		CancellationToken cancellationToken)
	{
		TerminalOutput.WriteOutput(parsedCommand, new StartedCommandEvent(-1));
		
		TerminalOutput.WriteOutput(
			parsedCommand, new StandardErrorCommandEvent("run source locally for terminal"));
			
		return Task.CompletedTask;
	}
	
	/* This method is in progress
	
	private Task ExecuteWebsiteCliAsync(
		TerminalCommandParsed parsedCommand,
		Action<CommandEvent> handleOutputAction,
		CancellationToken cancellationToken)
	{
		TerminalOutput.WriteOutput(parsedCommand, new StartedCommandEvent(-1));
	
		switch (parsedCommand.TargetFileName)
		{
			case TerminalWebsiteFacts.TargetFileNames.DOT_NET:
				return DotNetWebsiteCliAsync(parsedCommand, handleOutputAction, cancellationToken);
		}
		
		TerminalOutput.WriteOutput(
			parsedCommand,
			new StandardErrorCommandEvent($"Target file name: '{parsedCommand.TargetFileName}' was not recognized."));
		return Task.CompletedTask;
	}
	*/
	
	private Task DotNetWebsiteCliAsync(
		TerminalCommandParsed parsedCommand,
		Action<CommandEvent> handleOutputAction,
		CancellationToken cancellationToken)
	{
		var argumentList = parsedCommand.Arguments.Trim().Split(' ');
	
		var firstArgument = argumentList.FirstOrDefault();
		
		if (string.IsNullOrWhiteSpace(firstArgument))
		{
			TerminalOutput.WriteOutput(parsedCommand, new StandardErrorCommandEvent($"firstArgument was null or whitespace."));
		}
	
		switch (firstArgument)
		{
			case TerminalWebsiteFacts.InitialArguments.RUN:
			{
				var projectPath = (string?)null;
			
				if (argumentList.Length == 2)
				{
					projectPath = argumentList[1];
				}
				else if (argumentList.Length == 3)
				{
					if (argumentList[1] != TerminalWebsiteFacts.Options.PROJECT)
					{
						TerminalOutput.WriteOutput(parsedCommand, new StandardErrorCommandEvent($"argumentList[1]:{argumentList[1]} != {TerminalWebsiteFacts.Options.PROJECT}"));
						return Task.CompletedTask;
					}
				
					projectPath = argumentList[2];
				}
				else
				{
					TerminalOutput.WriteOutput(parsedCommand, new StandardErrorCommandEvent($"Bad argument count: '{argumentList.Length}'."));
					return Task.CompletedTask;
				}
			
				TerminalOutput.WriteOutput(parsedCommand, new StandardErrorCommandEvent($"projectPath:{projectPath}"));
				return Task.CompletedTask;
			}
			default:
			{
				TerminalOutput.WriteOutput(parsedCommand, new StandardErrorCommandEvent($"First argument: '{firstArgument}' was not recognized."));
				return Task.CompletedTask;
			}
		}
		
		throw new LuthetusIdeException($"The method '{nameof(DotNetWebsiteCliAsync)}' should return on each branch of the switch statement.");
	}

	public void KillProcess()
    {
        _commandCancellationTokenSource.Cancel();
        _commandCancellationTokenSource = new();
        DispatchNewStateKey();
    }

	private void DispatchNewStateKey()
    {
        // _dispatcher.Dispatch(new TerminalState.NotifyStateChangedAction(Key));
    }

    public IBackgroundTaskGroup? EarlyBatchOrDefault(IBackgroundTaskGroup oldEvent)
    {
        return null;
    }

    public ValueTask HandleEvent(CancellationToken cancellationToken)
    {
        TerminalWorkKind workKind;

        lock (_workLock)
        {
            if (!_workKindQueue.TryDequeue(out workKind))
                return ValueTask.CompletedTask;
        }

        switch (workKind)
        {
            case TerminalWorkKind.Command:
            {
                var args = _queue_general_TerminalCommandRequest.Dequeue();
                return DoCommand(args);
            }
            default:
            {
                Console.WriteLine($"{nameof(TerminalWebsite)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
            }
        }
    }

    public void Dispose()
    {
    	TerminalInput.Dispose();
		TerminalOutput.Dispose();
		// Input and output are dependent on 'TerminalInteractive'.
		// Therefore, dispose it last.
		TerminalInteractive.Dispose();
    }
}

