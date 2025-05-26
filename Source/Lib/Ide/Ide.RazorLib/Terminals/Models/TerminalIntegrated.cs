using System.Reactive.Linq;
using System.Text;
using CliWrap;
using CliWrap.EventStream;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalIntegrated : ITerminal, IBackgroundTaskGroup
{
	private readonly BackgroundTaskService _backgroundTaskService;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly INotificationService _notificationService;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly string _pathToShellExecutable;

	public TerminalIntegrated(
		string displayName,
		Func<TerminalIntegrated, ITerminalInteractive> terminalInteractiveFactory,
		Func<TerminalIntegrated, ITerminalInput> terminalInputFactory,
		Func<TerminalIntegrated, ITerminalOutput> terminalOutputFactory,
		BackgroundTaskService backgroundTaskService,
		ICommonComponentRenderers commonComponentRenderers,
		INotificationService notificationService,
		IEnvironmentProvider environmentProvider,
		string pathToShellExecutable)
	{
		DisplayName = displayName;
		TerminalInteractive = terminalInteractiveFactory.Invoke(this);
		TerminalInput = terminalInputFactory.Invoke(this);
		TerminalOutput = terminalOutputFactory.Invoke(this);
		
		_backgroundTaskService = backgroundTaskService;
		_commonComponentRenderers = commonComponentRenderers;
		_notificationService = notificationService;
		_environmentProvider = environmentProvider;
		_pathToShellExecutable = pathToShellExecutable;
	}

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();
    public string Name { get; } = nameof(TerminalIntegrated);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<TerminalWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();

    private CancellationTokenSource _commandCancellationTokenSource = new();
	private Task? _shellTask;
	private Command? _shellCliWrapCommand;

	public string DisplayName { get; }
	public ITerminalInteractive TerminalInteractive { get; }
	public ITerminalInput TerminalInput { get; }
	public ITerminalOutput TerminalOutput { get; }

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
            _backgroundTaskService.Indefinite_EnqueueGroup(this);
        }
    }

    public ValueTask DoCommand(TerminalCommandRequest terminalCommandRequest)
    {
        return HandleCommand(terminalCommandRequest);
    }

    public Task EnqueueCommandAsync(TerminalCommandRequest terminalCommandRequest)
    {
		return _backgroundTaskService.Indefinite_EnqueueAsync(
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
    	
    	var cliWrapCommand = Cli.Wrap(parsedCommand.TargetFileName);

		cliWrapCommand = cliWrapCommand.WithWorkingDirectory(TerminalInteractive.WorkingDirectory);

		if (!string.IsNullOrWhiteSpace(parsedCommand.Arguments))
			cliWrapCommand = cliWrapCommand.WithArguments(parsedCommand.Arguments);
		
		// TODO: Decide where to put invocation of 'parsedCommand.SourceTerminalCommandRequest.BeginWithFunc'...
		//       ...and invocation of 'cliWrapCommand'
		//       and invocation of 'parsedCommand.SourceTerminalCommandRequest.ContinueWithFunc'
		//       |
		// 	  This comment is referring to the 'try/catch' block logic.
		//       If the 'BeginWithFunc' throws an exception should the 'cliWrapCommand' run?
		//       If the 'cliWrapCommand' throws an exception should the 'ContinueWithFunc' run?
		
		try
		{
			HasExecutingProcess = true;
		
			if (parsedCommand.SourceTerminalCommandRequest.BeginWithFunc is not null)
			{
				await parsedCommand.SourceTerminalCommandRequest.BeginWithFunc
					.Invoke(parsedCommand)
					.ConfigureAwait(false);
			}
			
			await cliWrapCommand
				.Observe(_commandCancellationTokenSource.Token)
				.ForEachAsync(HandleOutput)
				.ConfigureAwait(false);
		}
		catch (Exception e)
		{
			// TODO: This will erroneously write 'StartedCommandEvent' out twice...
			//       ...unless a check is added to see WHEN the exception was thrown.
			TerminalOutput.WriteOutput(
				parsedCommand,
				new StartedCommandEvent(-1));
		
			TerminalOutput.WriteOutput(
				parsedCommand,
				new StandardErrorCommandEvent(
					parsedCommand.SourceTerminalCommandRequest.CommandText +
					" threw an exception" +
					"\n"));
		
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

	public void KillProcess()
    {
        _commandCancellationTokenSource.Cancel();
        _commandCancellationTokenSource = new();
        DispatchNewStateKey();
    }
    
    public void Start()
    {
    	_shellTask = Task.Run(async () =>
		{
			var bufferStdIn = new StringBuilder("abc");
			var bufferStdOut = new StringBuilder();
			var bufferStdError = new StringBuilder();
		
			try
			{
				var terminalCommandRequest = new TerminalCommandRequest(
		    		$"{_pathToShellExecutable} -i",
					_environmentProvider.HomeDirectoryAbsolutePath.Value);
		    	
		    	var parsedCommand = await TerminalInteractive.TryHandleCommand(terminalCommandRequest);
		    	ActiveTerminalCommandParsed = parsedCommand;
		
				if (parsedCommand is null)
					return;
					
				TerminalOutput.WriteOutput(
					parsedCommand,
					new StartedCommandEvent(-1));
					
				var pipeFilePath = _environmentProvider.JoinPaths(
					_environmentProvider.SafeRoamingApplicationDataDirectoryAbsolutePath.Value,
					"terminal-test.txt");
					
				_shellCliWrapCommand = "abc" | Cli
					.Wrap(parsedCommand.TargetFileName)
					.WithWorkingDirectory(terminalCommandRequest.WorkingDirectory) |
					(PipeTarget.ToStringBuilder(bufferStdOut), PipeTarget.ToStringBuilder(bufferStdError));
		
				if (!string.IsNullOrWhiteSpace(parsedCommand.Arguments))
					_shellCliWrapCommand = _shellCliWrapCommand.WithArguments(parsedCommand.Arguments);
		    
				Console.WriteLine("before shell");
				
				await _shellCliWrapCommand
					.ExecuteAsync(_commandCancellationTokenSource.Token);
					
				Console.WriteLine("after shell");
			}
			catch (Exception e)
			{
				Console.WriteLine("exception shell");
				NotificationHelper.DispatchError("Terminal Exception", e.ToString(), _commonComponentRenderers, _notificationService, TimeSpan.FromSeconds(14));
			}
			finally
			{
				Console.WriteLine($"bufferStdOut: {bufferStdOut}");
				Console.WriteLine($"bufferStdError: {bufferStdError}");
			}
		});
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
                Console.WriteLine($"{nameof(TerminalIntegrated)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
            }
        }
    }

    public void Dispose()
    {
    	KillProcess();
    
    	TerminalInput.Dispose();
		TerminalOutput.Dispose();
		// Input and output are dependent on 'TerminalInteractive'.
		// Therefore, dispose it last.
		TerminalInteractive.Dispose();
    }
}
