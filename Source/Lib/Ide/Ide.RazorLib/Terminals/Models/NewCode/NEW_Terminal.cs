using System.Collections.Immutable;
using System.Reactive.Linq;
using CliWrap;
using CliWrap.EventStream;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib;
using Luthetus.Ide.RazorLib.Terminals.States;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>
/// This implementation of <see cref="ITerminal"/> is a "blank slate".
/// </summary>
public class NEW_Terminal : ITerminal
{
	private readonly IBackgroundTaskService _backgroundTaskService;
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly IDispatcher _dispatcher;

	public NEW_Terminal(
		string displayName,
		Func<NEW_Terminal, ITerminalInteractive> terminalInteractiveFactory,
		Func<NEW_Terminal, ITerminalInput> terminalInputFactory,
		Func<NEW_Terminal, ITerminalOutput> terminalOutputFactory,
		IBackgroundTaskService backgroundTaskService,
		ICommonComponentRenderers commonComponentRenderers,
		IDispatcher dispatcher)
	{
		DisplayName = displayName;
		TerminalInteractive = terminalInteractiveFactory.Invoke(this);
		TerminalInput = terminalInputFactory.Invoke(this);
		TerminalOutput = terminalOutputFactory.Invoke(this);
		
		_backgroundTaskService = backgroundTaskService;
		_commonComponentRenderers = commonComponentRenderers;
		_dispatcher = dispatcher;
	}

	public string DisplayName { get; }
	public ITerminalInteractive TerminalInteractive { get; }
	public ITerminalInput TerminalInput { get; }
	public ITerminalOutput TerminalOutput { get; }

	private CancellationTokenSource _commandCancellationTokenSource = new();

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
    
    public void EnqueueClear()
    {
    	EnqueueCommand(new TerminalCommandRequest("clear", null));
    }

    private async Task HandleCommand(TerminalCommandRequest terminalCommandRequest)
    {
    	var parsedCommand = await TerminalInteractive.TryHandleCommand(terminalCommandRequest);
    	ActiveTerminalCommandParsed = parsedCommand;

		if (parsedCommand is null)
			return;
    	
    	var cliWrapCommand = Cli.Wrap(parsedCommand.TargetFileName);

		cliWrapCommand = cliWrapCommand.WithWorkingDirectory(TerminalInteractive.WorkingDirectory);

		if (!string.IsNullOrWhiteSpace(parsedCommand.Arguments))
			cliWrapCommand = cliWrapCommand.WithArguments(parsedCommand.Arguments);
		
		try
		{
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
				
			if (parsedCommand.SourceTerminalCommandRequest.ContinueWithFunc is not null)
			{
				await parsedCommand.SourceTerminalCommandRequest.ContinueWithFunc
					.Invoke(parsedCommand)
					.ConfigureAwait(false);
			}
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

	private void DispatchNewStateKey()
    {
        // _dispatcher.Dispatch(new TerminalState.NotifyStateChangedAction(Key));
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
