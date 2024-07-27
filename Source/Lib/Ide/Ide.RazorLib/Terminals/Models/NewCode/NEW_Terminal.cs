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

	public NEW_Terminal(
		string displayName,
		Func<NEW_Terminal, ITerminalInteractive> terminalInteractiveFactory,
		Func<NEW_Terminal, ITerminalInput> terminalInputFactory,
		Func<NEW_Terminal, ITerminalOutput> terminalOutputFactory,
		IBackgroundTaskService backgroundTaskService)
	{
		DisplayName = displayName;
		TerminalInteractive = terminalInteractiveFactory.Invoke(this);
		TerminalInput = terminalInputFactory.Invoke(this);
		TerminalOutput = terminalOutputFactory.Invoke(this);
		
		_backgroundTaskService = backgroundTaskService;
	}

	public string DisplayName { get; }
	public ITerminalInteractive TerminalInteractive { get; }
	public ITerminalInput TerminalInput { get; }
	public ITerminalOutput TerminalOutput { get; }

	private CancellationTokenSource _commandCancellationTokenSource = new();

    public Key<ITerminal> Key { get; init; } = Key<ITerminal>.NewKey();
    public TerminalCommand? ActiveTerminalCommand { get; private set; }

	/// <summary>NOTE: the following did not work => _process?.HasExited ?? false;</summary>
    public bool HasExecutingProcess { get; private set; }

    public void EnqueueCommand(string commandText)
    {
    	/*
		_backgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			BlockingBackgroundTaskWorker.GetQueueKey(),
			"Enqueue Command",
			() => HandleCommand(terminalCommand));
		*/
    }

    private async Task HandleCommand(string commandText)
    {
    	// ITerminalOutputPipe.OnHandleCommandStarting(...)
    	
    
    	/*
    	var wasInteractiveCommand = TerminalInteractive.TryHandleCommand(commandText);

		_terminalCommandsHistory.Add(terminalCommand);
		ActiveTerminalCommand = terminalCommand;

		var command = Cli.Wrap(terminalCommand.FormattedCommand.TargetFileName);

		if (terminalCommand.FormattedCommand.ArgumentsList.Any())
		{
			if (terminalCommand.FormattedCommand.HACK_ArgumentsString is null)
				command = command.WithArguments(terminalCommand.FormattedCommand.ArgumentsList);
			else
				command = command.WithArguments(terminalCommand.FormattedCommand.HACK_ArgumentsString);
		}

		if (terminalCommand.ChangeWorkingDirectoryTo is not null)
			command = command.WithWorkingDirectory(terminalCommand.ChangeWorkingDirectoryTo);
		else if (WorkingDirectoryAbsolutePathString is not null)
			command = command.WithWorkingDirectory(WorkingDirectoryAbsolutePathString);

		try
		{
			var terminalCommandKey = terminalCommand.TerminalCommandKey;
			terminalCommand.TextSpan = null;
			terminalCommand.WasStarted = true;
			await terminalCommand.InvokeStateChangedCallbackFunc();

			HasExecutingProcess = true;
			DispatchNewStateKey();

			if (terminalCommand.BeginWith is not null)
				await terminalCommand.BeginWith.Invoke().ConfigureAwait(false);

			// It is important to invoke 'OnAfterCommandStarted' after 'terminalCommand.BeginWith'
			if (terminalCommand.OutputParser is not null)
			{
				await terminalCommand.OutputParser
					.OnAfterCommandStarted(terminalCommand)
					.ConfigureAwait(false);
			}

			var terminalCommandBoundary = new TerminalCommandBoundary();
			var outputOffset = 0;

			await command
				.Observe(_commandCancellationTokenSource.Token)
				.ForEachAsync(TerminalOutput.OnOutput)
				.ConfigureAwait(false);
		}
		catch (Exception e)
		{
			NotificationHelper.DispatchError("Terminal Exception", e.ToString(), _commonComponentRenderers, _dispatcher, TimeSpan.FromSeconds(14));
		}
		finally
		{
			terminalCommand.IsCompleted = true;
			await terminalCommand.InvokeStateChangedCallbackFunc();
			HasExecutingProcess = false;
			WriteWorkingDirectory();
			DispatchNewStateKey();

			// It is important to invoke 'OnAfterCommandFinished' prior to 'terminalCommand.ContinueWith'
			if (terminalCommand.OutputParser is not null)
			{
				// TODO: If one's 'OutputParser' throws an exception here, then the 'ContinueWith'...
				//       ...will not run.
				//       |
				//       So, what is the desired behavior? Should a try block be used here?
				await terminalCommand.OutputParser
					.OnAfterCommandFinished(terminalCommand)
					.ConfigureAwait(false);
			}

			if (terminalCommand.ContinueWith is not null)
			{
				await terminalCommand.ContinueWith
					.Invoke()
					.ConfigureAwait(false);
			}
		}
		*/
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
