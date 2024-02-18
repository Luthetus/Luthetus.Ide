using CliWrap;
using CliWrap.EventStream;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Ide.RazorLib.States.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Text;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalSession
{
    private readonly IDispatcher _dispatcher;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
    private readonly List<TerminalCommand> _terminalCommandsHistory = new();
    private readonly object _standardOutBuilderMapLock = new();

    private CancellationTokenSource _commandCancellationTokenSource = new();

    private readonly ConcurrentQueue<TerminalCommand> _terminalCommandsConcurrentQueue = new();

    // Here there is a map from 'Key<TerminalCommand>' to 'StringBuilder'
	// I'd like to change 'StringBuilder' to List<string>
	//
	// I know that the StringBuilder, as text comes from the terminal output,
	// that the terminal output provides text line by line.
	// Each line is a separate invocation of a method.
	//
	// I essentially say,
	// OnLineOutput(string text) => stringBuilder.Append($"text\n")
	//
	// Each method invocation just appends the line with a newline character.
	// So this should be an easy change. Instead of appending to a stringBuilder
	// I can append to a list of string.
	//
	// Funny enough the goto definition keymap worked. { F12 } is the keymap
	//
	// I'm going to iterate over the "Find" results. And makesure that everything seems to be in place.
    private readonly Dictionary<Key<TerminalCommand>, List<string>> _standardOutBuilderMap = new();

    public TerminalSession(
        string? workingDirectoryAbsolutePathString,
        IDispatcher dispatcher,
        IBackgroundTaskService backgroundTaskService,
        ILuthetusCommonComponentRenderers commonComponentRenderers)
    {
        _dispatcher = dispatcher;
        _backgroundTaskService = backgroundTaskService;
        _commonComponentRenderers = commonComponentRenderers;
        WorkingDirectoryAbsolutePathString = workingDirectoryAbsolutePathString;
    }

    public Key<TerminalSession> TerminalSessionKey { get; init; } = Key<TerminalSession>.NewKey();

    public ResourceUri ResourceUri => new($"__LUTHETUS_{TerminalSessionKey.Guid}__");
    public Key<TextEditorViewModel> TextEditorViewModelKey => new(TerminalSessionKey.Guid);

    public string? WorkingDirectoryAbsolutePathString { get; private set; }

    public TerminalCommand? ActiveTerminalCommand { get; private set; }

    public ImmutableArray<TerminalCommand> TerminalCommandsHistory => _terminalCommandsHistory.ToImmutableArray();

    /// <summary>NOTE: the following did not work => _process?.HasExited ?? false;</summary>
    public bool HasExecutingProcess { get; private set; }

	// Now that the internals of TerminalSession have been changed.
	// How do Blazor components acccess the standard out?
	//
	// Its with the ReadStandardOut methods
	// So we have to track down the references to these methods
    public string ReadStandardOut()
    {
		var output = string.Empty;

		lock(_standardOutBuilderMapLock)
		{
			// Okay, should the _standardOutBuilderMap's List<string> store the
			// line ending character foreach entry?
			// 
			// I'm going to say no, do not store the line ending character.
			//
			// As such, this method ReadStandardOut() needs to be changed, so that it can combine
			// every terminal command's output into a single string.
			//
			// The '.Select' lambda uses the variable 'x'.
			// After having made our changes, the variable 'x' is now a List<string>, NOT a StringBuilder.
			//
			// The following line isn't the nicest thing to read.
			// But I am content with it for now.
			//
			// I'm coming back to this code, because I forgot to add the line endings.
			// The line endings aren't stored, they must be added when joining the entries.
			//
			// I'm thinking about what the first step is here. 'aaa' is just a temporary variable name while I
			// work out the details

			var entireStdOutStringBuilder = new StringBuilder();

			foreach (var strList in _standardOutBuilderMap.Values)
			{
				var perCommandStringBuilder = new StringBuilder();
	
				foreach (var str in strList)
				{
					// I'm not sure what the code is for "Environment.NewLine" so I'm { Ctrl + Shift + f }ing
					// that text to see if I used it elsewhere.
					perCommandStringBuilder.Append($"{perCommandStringBuilder}{Environment.NewLine}");
				}

				entireStdOutStringBuilder.Append(perCommandStringBuilder.ToString());
			}

			output = entireStdOutStringBuilder.ToString();
		}

        return output;
    }

	// Here is the second ReadStandardOut method
	//
	// The first one was reading the entirety of the terminal
	//
	// this one reads the specific command output
    public string? ReadStandardOut(Key<TerminalCommand> terminalCommandKey)
    {
		var output = (string?)null;

		lock(_standardOutBuilderMapLock)
		{
			if (_standardOutBuilderMap.TryGetValue(terminalCommandKey, out var strList))
			{
				var perCommandStringBuilder = new StringBuilder();
	
				foreach (var str in strList)
				{
					perCommandStringBuilder.Append($"{perCommandStringBuilder}{Environment.NewLine}");
				}

				output = perCommandStringBuilder.ToString();
			}
		}

        return output;
    }

	/// <summary>
	/// Returns the output that occurred in the terminal session
	/// </summary>
	public List<string>? GetStandardOut()
	{
		// I forgot to put 'null'
		var allOutput = (List<string>?)null;

		lock(_standardOutBuilderMapLock)
		{
			allOutput = _standardOutBuilderMap
				.SelectMany(kvp => GetStandardOut(kvp.Key))
				.ToList();
		}

        return allOutput;

		// Before continuing, I want to get the program to run without errors.
		// We have changed nothing so far in terms of public API.
		// Since the ReadStandardOut input and output were untouched.
		//
		// I should stop this recording and start a new one.
		// I'll do so in a second.
		// okay doing
	}

	/// <summary>
	/// Returns the output that occurred as a result of a specific command
	/// </summary>
    public List<string>? GetStandardOut(Key<TerminalCommand> terminalCommandKey)
	{
		var output = (List<string>?)null;

		lock(_standardOutBuilderMapLock)
		{
			if (_standardOutBuilderMap.TryGetValue(terminalCommandKey, out var strList))
				output = new List<string>(strList); // Shallow copy to hide private memory location
		}

        return output;
	}

    public Task EnqueueCommandAsync(TerminalCommand terminalCommand)
    {
		// This is the 'EnqueueCommandAsync' method, that runs terminal command
        var queueKey = BlockingBackgroundTaskWorker.GetQueueKey();

        _backgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), queueKey,
            "Enqueue Command",
            async () =>
            {
                if (terminalCommand.ChangeWorkingDirectoryTo is not null)
                    WorkingDirectoryAbsolutePathString = terminalCommand.ChangeWorkingDirectoryTo;

                if (terminalCommand.FormattedCommand.TargetFileName == "cd" &&
                    terminalCommand.FormattedCommand.ArgumentsList.Any())
                {
                    // TODO: Don't keep this logic as it is hacky. I'm trying to set myself up to be able to run "gcc" to compile ".c" files. Then I can work on adding symbol related logic like "go to definition" or etc.
                    WorkingDirectoryAbsolutePathString = terminalCommand.FormattedCommand.ArgumentsList.ElementAt(0);
                }

                _terminalCommandsHistory.Add(terminalCommand);
                ActiveTerminalCommand = terminalCommand;

                var command = Cli.Wrap(terminalCommand.FormattedCommand.TargetFileName);

                if (terminalCommand.FormattedCommand.ArgumentsList.Any())
                    command = command.WithArguments(terminalCommand.FormattedCommand.ArgumentsList);

                if (terminalCommand.ChangeWorkingDirectoryTo is not null)
                    command = command.WithWorkingDirectory(terminalCommand.ChangeWorkingDirectoryTo);
                else if (WorkingDirectoryAbsolutePathString is not null)
                    command = command.WithWorkingDirectory(WorkingDirectoryAbsolutePathString);

                // Push-based event stream
                {
                    var terminalCommandKey = terminalCommand.TerminalCommandKey;
					
					// Here, immediately prior to starting the terminal command,
					// a StringBuilder is being made for the respective terminal command to write to.
					//
					// I'll change this to make a new List<string>()
					lock(_standardOutBuilderMapLock)
					{
						// the _standardOutBuilderMap field declaration needs to be updated accordingly.
						_standardOutBuilderMap.TryAdd(terminalCommand.TerminalCommandKey, new List<string>());
					}

                    HasExecutingProcess = true;
                    DispatchNewStateKey();

                    try
                    {
						if (terminalCommand.BeginWith is not null)
                            await terminalCommand.BeginWith.Invoke(); // Actually start the terminal command here

                        await command.Observe(_commandCancellationTokenSource.Token)
                            .ForEachAsync(cmdEvent =>
                            {
								var output = (string?)null; // a variable for storing the output

                                switch (cmdEvent)
                                {
                                    case StartedCommandEvent started:
                                        output = $"> {WorkingDirectoryAbsolutePathString} (PID:{started.ProcessId}) {terminalCommand.FormattedCommand.Value}";
                                        break;
                                    case StandardOutputCommandEvent stdOut:
                                        output = $"{stdOut.Text}";
                                        break;
                                    case StandardErrorCommandEvent stdErr:
                                        output = $"Err> {stdErr.Text}";
                                        break;
                                    case ExitedCommandEvent exited:
                                        output = $"Process exited; Code: {exited.ExitCode}";
                                        break;
                                }

								if (output is not null)
								{
									lock(_standardOutBuilderMapLock)
									{
										// Append the line here
										// We need to change this to add an entry to the List<string> now
										_standardOutBuilderMap[terminalCommandKey].Add(output);
									}
								}

                                DispatchNewStateKey();
                            });
                    }
                    catch (Exception e)
                    {
                        NotificationHelper.DispatchError("Terminal Exception", e.ToString(), _commonComponentRenderers, _dispatcher, TimeSpan.FromSeconds(14));
                    }
                    finally
                    {
                        HasExecutingProcess = false;
                        DispatchNewStateKey();

                        if (terminalCommand.ContinueWith is not null)
                            await terminalCommand.ContinueWith.Invoke();
                    }
                }
            });

        return Task.CompletedTask;
    }

    public void ClearStandardOut()
    {
		lock(_standardOutBuilderMapLock)
		{
			foreach (var stringBuilder in _standardOutBuilderMap.Values)
	        {
	            stringBuilder.Clear(); // .Clear() is a method on List(s) too
	        }
		}

        DispatchNewStateKey();
    }

	public void ClearStandardOut(Key<TerminalCommand> terminalCommandKey)
    {
		lock(_standardOutBuilderMapLock)
		{
			_standardOutBuilderMap[terminalCommandKey].Clear();
		}

        DispatchNewStateKey();
    }

    public void KillProcess()
    {
        _commandCancellationTokenSource.Cancel();
        _commandCancellationTokenSource = new();

        DispatchNewStateKey();
    }

    private void DispatchNewStateKey()
    {
        _dispatcher.Dispatch(new TerminalSessionWasModifiedState.SetTerminalSessionStateKeyAction(
            TerminalSessionKey,
            Key<StateRecord>.NewKey()));
    }
}