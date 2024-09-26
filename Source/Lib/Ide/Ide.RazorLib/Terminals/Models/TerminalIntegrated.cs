using System.Reactive.Linq;
using CliWrap;
using CliWrap.EventStream;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalIntegrated : ITerminal
{
	private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly IDispatcher _dispatcher;
	private readonly IEnvironmentProvider _environmentProvider;
	private readonly string _pathToShellExecutable;

	public TerminalIntegrated(
		string displayName,
		Func<TerminalIntegrated, ITerminalInteractive> terminalInteractiveFactory,
		Func<TerminalIntegrated, ITerminalInput> terminalInputFactory,
		Func<TerminalIntegrated, ITerminalOutput> terminalOutputFactory,
		IBackgroundTaskService backgroundTaskService,
		ICommonComponentRenderers commonComponentRenderers,
		IDispatcher dispatcher,
		IEnvironmentProvider environmentProvider,
		string pathToShellExecutable)
	{
		DisplayName = displayName;
		TerminalInteractive = terminalInteractiveFactory.Invoke(this);
		TerminalInput = terminalInputFactory.Invoke(this);
		TerminalOutput = terminalOutputFactory.Invoke(this);
		
		_backgroundTaskService = backgroundTaskService;
		_commonComponentRenderers = commonComponentRenderers;
		_dispatcher = dispatcher;
		_environmentProvider = environmentProvider;
		_pathToShellExecutable = pathToShellExecutable;
	}
	
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

    public void EnqueueCommand(TerminalCommandRequest terminalCommandRequest)
    {
		_backgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			BlockingBackgroundTaskWorker.GetQueueKey(),
			"Enqueue Command",
			() => HandleCommand(terminalCommandRequest));
    }
    
    public Task EnqueueCommandAsync(TerminalCommandRequest terminalCommandRequest)
    {
		return _backgroundTaskService.EnqueueAsync(
			Key<IBackgroundTask>.NewKey(),
			BlockingBackgroundTaskWorker.GetQueueKey(),
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

    private async Task HandleCommand(TerminalCommandRequest terminalCommandRequest)
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
		
			NotificationHelper.DispatchError("Terminal Exception", e.ToString(), _commonComponentRenderers, _dispatcher, TimeSpan.FromSeconds(14));
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
					
				_shellCliWrapCommand = Cli
					.Wrap(parsedCommand.TargetFileName)
					.WithWorkingDirectory(terminalCommandRequest.WorkingDirectory);
		
				if (!string.IsNullOrWhiteSpace(parsedCommand.Arguments))
					_shellCliWrapCommand = _shellCliWrapCommand.WithArguments(parsedCommand.Arguments);
		    
				Console.WriteLine("before shell");
				
				await _shellCliWrapCommand
					.Observe(_commandCancellationTokenSource.Token)
					.ForEachAsync(HandleOutput);
					
				Console.WriteLine("after shell");
			}
			catch (Exception e)
			{
				NotificationHelper.DispatchError("Terminal Exception", e.ToString(), _commonComponentRenderers, _dispatcher, TimeSpan.FromSeconds(14));
			}
		});
    }

	private void DispatchNewStateKey()
    {
        // _dispatcher.Dispatch(new TerminalState.NotifyStateChangedAction(Key));
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
