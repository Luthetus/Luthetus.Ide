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

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public partial class Terminal
{
    private readonly IDispatcher _dispatcher;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly ICompilerServiceRegistry _compilerServiceRegistry;
    private readonly List<TerminalCommand> _terminalCommandsHistory = new();

	/// <summary>
	/// This factory method exists because the terminal has async methods
	/// it invokes immediately after being constructed.
	/// </summary>
	public static Task<Terminal> Factory(
        string displayName,
        string? workingDirectoryAbsolutePathString,
        IDispatcher dispatcher,
        IBackgroundTaskService backgroundTaskService,
        ITextEditorService textEditorService,
        ICommonComponentRenderers commonComponentRenderers,
        ICompilerServiceRegistry compilerServiceRegistry,
		Key<Terminal> terminalKey)
	{
		var terminal = new Terminal(
			displayName,
	        dispatcher,
	        backgroundTaskService,
	        textEditorService,
	        commonComponentRenderers,
	        compilerServiceRegistry)
			{
				Key = terminalKey
			};

		terminal.CreateTextEditor();
		terminal.SetWorkingDirectoryAbsolutePathString(workingDirectoryAbsolutePathString);
		return Task.FromResult(terminal);
	}

    /// <summary>
    /// One should use the <see cref="Factory"/> method.
    /// </summary>
    private Terminal(
        string displayName,
        IDispatcher dispatcher,
        IBackgroundTaskService backgroundTaskService,
        ITextEditorService textEditorService,
        ICommonComponentRenderers commonComponentRenderers,
        ICompilerServiceRegistry compilerServiceRegistry)
    {
        _dispatcher = dispatcher;
        _backgroundTaskService = backgroundTaskService;
        _textEditorService = textEditorService;
        _commonComponentRenderers = commonComponentRenderers;
        _compilerServiceRegistry = compilerServiceRegistry;

        DisplayName = displayName;
        ResourceUri = new(ResourceUriFacts.Terminal_ReservedResourceUri_Prefix + Key.Guid.ToString());
    }

	private CancellationTokenSource _commandCancellationTokenSource = new();
    private string? _previousWorkingDirectoryAbsolutePathString;
    private string? _workingDirectoryAbsolutePathString;

    public Key<Terminal> Key { get; init; } = Key<Terminal>.NewKey();
    public TerminalCommand? ActiveTerminalCommand { get; private set; }

	/// <summary>NOTE: the following did not work => _process?.HasExited ?? false;</summary>
    public bool HasExecutingProcess { get; private set; }

    public string DisplayName { get; }

    public string? WorkingDirectoryAbsolutePathString => _workingDirectoryAbsolutePathString;

    public ImmutableArray<TerminalCommand> TerminalCommandsHistory => _terminalCommandsHistory.ToImmutableArray();

	public void SetWorkingDirectoryAbsolutePathString(string? value)
	{
		_previousWorkingDirectoryAbsolutePathString = _workingDirectoryAbsolutePathString;
        _workingDirectoryAbsolutePathString = value;

        if (_previousWorkingDirectoryAbsolutePathString != _workingDirectoryAbsolutePathString)
            WriteWorkingDirectory(true);
	}

    public void EnqueueCommand(TerminalCommand terminalCommand)
    {
		_backgroundTaskService.Enqueue(
			Key<IBackgroundTask>.NewKey(),
			BlockingBackgroundTaskWorker.GetQueueKey(),
			"Enqueue Command",
			() => HandleCommand(terminalCommand));
    }

    private async Task HandleCommand(TerminalCommand terminalCommand)
    {
    	if (terminalCommand.OutputBuilder is null)
			ClearTerminal();

		if (terminalCommand.ChangeWorkingDirectoryTo is not null)
			SetWorkingDirectoryAbsolutePathString(terminalCommand.ChangeWorkingDirectoryTo);

		if (terminalCommand.FormattedCommand.TargetFileName == "cd")
		{
			// TODO: Don't keep this logic as it is hacky. I'm trying to set myself up to be able to run "gcc" to compile ".c" files. Then I can work on adding symbol related logic like "go to definition" or etc.
			if (terminalCommand.FormattedCommand.HACK_ArgumentsString is not null)
				SetWorkingDirectoryAbsolutePathString(terminalCommand.FormattedCommand.HACK_ArgumentsString);
			else if (terminalCommand.FormattedCommand.ArgumentsList.Any())
				SetWorkingDirectoryAbsolutePathString(terminalCommand.FormattedCommand.ArgumentsList.ElementAt(0));

			return;
		}

		if (terminalCommand.FormattedCommand.TargetFileName == "clear")
		{
			ClearTerminal();
			WriteWorkingDirectory();
			return;
		}

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

			await command.Observe(_commandCancellationTokenSource.Token)
				.ForEachAsync(async cmdEvent =>
				{
					var output = (string?)null;

					switch (cmdEvent)
					{
						case StartedCommandEvent started:
							// TODO: If the source of the terminal command is a user having...
							//       ...typed themselves, then hitting enter, do not write this out.
							//       |
							//       This is here for when the command was started programmatically
							//       without a user typing into the terminal.
							output = $"{terminalCommand.FormattedCommand.Value}\n";
							break;
						case StandardOutputCommandEvent stdOut:
							output = $"{stdOut.Text}\n";
							break;
						case StandardErrorCommandEvent stdErr:
							output = $"{stdErr.Text}\n";
							break;
						case ExitedCommandEvent exited:
							output = $"Process exited; Code: {exited.ExitCode}\n";
							break;
					}

					if (output is not null)
					{
						var outputTextSpanList = new List<TextEditorTextSpan>();

						if (terminalCommand.OutputParser is not null)
						{
							outputTextSpanList = terminalCommand.OutputParser.OnAfterOutputLine(
								terminalCommand,
								output);
						}
						
						if (terminalCommand.OutputBuilder is null)
						{
							TerminalOnOutput(
								outputOffset,
								output,
								outputTextSpanList,
								terminalCommand,
								terminalCommandBoundary);
	
							outputOffset += output.Length;
						}
						else
						{
							terminalCommand.OutputBuilder.Append(output);
							terminalCommand.TextSpanList = outputTextSpanList;
						}
					}

					DispatchNewStateKey();
				}).ConfigureAwait(false);
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
	}

	public void KillProcess()
    {
        _commandCancellationTokenSource.Cancel();
        _commandCancellationTokenSource = new();
        DispatchNewStateKey();
    }

	private void DispatchNewStateKey()
    {
        _dispatcher.Dispatch(new TerminalState.NotifyStateChangedAction(Key));
    }
}