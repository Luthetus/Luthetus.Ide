using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using CliWrap;
using CliWrap.EventStream;
using Fluxor;
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
    private readonly ConcurrentQueue<TerminalCommand> _terminalCommandsConcurrentQueue = new();
    private readonly Dictionary<Key<TerminalCommand>, TerminalSessionOutput> _standardOutBuilderMap = new();

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

	private CancellationTokenSource _commandCancellationTokenSource = new();

    public Key<TerminalSession> TerminalSessionKey { get; init; } = Key<TerminalSession>.NewKey();
    public string? WorkingDirectoryAbsolutePathString { get; private set; }
    public TerminalCommand? ActiveTerminalCommand { get; private set; }
	/// <summary>NOTE: the following did not work => _process?.HasExited ?? false;</summary>
    public bool HasExecutingProcess { get; private set; }

    public ImmutableArray<TerminalCommand> TerminalCommandsHistory => _terminalCommandsHistory.ToImmutableArray();
	public ResourceUri ResourceUri => new($"__LUTHETUS_{TerminalSessionKey.Guid}__");
    public Key<TextEditorViewModel> TextEditorViewModelKey => new(TerminalSessionKey.Guid);

	/// <summary>
	/// Returns the output that occurred in the terminal session
	/// </summary>
    public string ReadStandardOut()
    {
		var output = string.Empty;

		lock(_standardOutBuilderMapLock)
		{
			var entireStdOutStringBuilder = new StringBuilder();

			foreach (var sessionOutput in _standardOutBuilderMap.Values)
			{
				var perCommandStringBuilder = new StringBuilder();
	
				foreach (var str in sessionOutput.TextLineList)
				{
					perCommandStringBuilder.Append(str);
				}

				entireStdOutStringBuilder.Append(perCommandStringBuilder.ToString());
			}

			output = entireStdOutStringBuilder.ToString();
		}

        return output;
    }

	/// <summary>
	/// Returns the output that occurred as a result of a specific command
	/// </summary>
    public string? ReadStandardOut(Key<TerminalCommand> terminalCommandKey)
    {
		var output = (string?)null;

		lock(_standardOutBuilderMapLock)
		{
			if (_standardOutBuilderMap.TryGetValue(terminalCommandKey, out var sessionOutput))
			{
				var perCommandStringBuilder = new StringBuilder();
	
				foreach (var str in sessionOutput.TextLineList)
				{
					perCommandStringBuilder.Append(str);
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
		var allOutput = (List<string>?)null;

		lock(_standardOutBuilderMapLock)
		{
			allOutput = _standardOutBuilderMap
				.SelectMany(kvp => GetStandardOut(kvp.Key))
				.ToList();
		}

        return allOutput;
	}

	/// <summary>
	/// Returns the output that occurred as a result of a specific command
	/// </summary>
    public List<string>? GetStandardOut(Key<TerminalCommand> terminalCommandKey)
	{
		var output = (List<string>?)null;

		lock(_standardOutBuilderMapLock)
		{
			if (_standardOutBuilderMap.TryGetValue(terminalCommandKey, out var sessionOutput))
				output = new List<string>(sessionOutput.TextLineList); // Shallow copy to hide private memory location
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

                // Event stream
                {
                    var terminalCommandKey = terminalCommand.TerminalCommandKey;
					
					// Here, immediately prior to starting the terminal command,
					// a StringBuilder is being made for the respective terminal command to write to.
					//
					// I'll change this to make a new List<string>()
					lock(_standardOutBuilderMapLock)
					{
						_standardOutBuilderMap.TryAdd(terminalCommand.TerminalCommandKey, new TerminalSessionOutput());
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
										lock(_standardOutBuilderMapLock)
										{
											output = $"> {terminalCommand.FormattedCommand.Value}";
	                                        _standardOutBuilderMap[terminalCommandKey].TextLineList.Add(output);

											output = $"> PID:{started.ProcessId} PWD:{WorkingDirectoryAbsolutePathString}";
	                                        _standardOutBuilderMap[terminalCommandKey].TextLineList.Add(output);

											output = null;
	                                    }

                                        break;
                                    case StandardOutputCommandEvent stdOut:
                                        output = $"{stdOut.Text}";
                                        break;
                                    case StandardErrorCommandEvent stdErr:
                                        output = $"{stdErr.Text}";
                                        break;
                                    case ExitedCommandEvent exited:
                                        output = $"Process exited; Code: {exited.ExitCode}";
                                        break;
                                }

								if (output is not null)
								{
									lock(_standardOutBuilderMapLock)
									{
                                        _standardOutBuilderMap[terminalCommandKey].TextLineList.Add(output);
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
        _dispatcher.Dispatch(new TerminalSessionState.NotifyStateChangedAction(TerminalSessionKey));
    }
}