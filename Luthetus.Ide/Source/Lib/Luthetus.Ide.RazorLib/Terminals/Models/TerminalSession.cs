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

    /// <summary>
    /// TODO: Prove that standard error is correctly being redirected to standard out
    /// </summary>
    private readonly Dictionary<Key<TerminalCommand>, StringBuilder> _standardOutBuilderMap = new();

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

    public string ReadStandardOut()
    {
		var output = string.Empty;

		lock(_standardOutBuilderMapLock)
		{
			output = string.Join(
	            string.Empty,
	            _standardOutBuilderMap.Select(x => x.Value.ToString()).ToArray());
		}

        return output;
    }

    public string? ReadStandardOut(Key<TerminalCommand> terminalCommandKey)
    {
		var output = (string?)null;

		lock(_standardOutBuilderMapLock)
		{
			if (_standardOutBuilderMap.TryGetValue(terminalCommandKey, out var outputBuilder))
	            output = outputBuilder.ToString();
		}

        return output;
    }

    public Task EnqueueCommandAsync(TerminalCommand terminalCommand)
    {
        var queueKey = BlockingBackgroundTaskWorker.GetQueueKey();

        if (TerminalSessionKey == TerminalSessionFacts.DEBUG_TERMINAL_SESSION_KEY)
        {
            // TODO: (2024-02-10) I'm experiencing issues with running both the...
            // ...program being debugged, and the debugger. As currently,
            // both will be put on the same queue, requiring the other to finish before it can begin.
            // 'TerminalSessionFacts.DEBUG_TERMINAL_SESSION_KEY' is a hacky way to get around this
            // issue for now. It will block the 'ContinuousBackgroundTaskWorker'.
            queueKey = ContinuousBackgroundTaskWorker.GetQueueKey();
        }

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
					
					lock(_standardOutBuilderMapLock)
					{
						_standardOutBuilderMap.TryAdd(terminalCommand.TerminalCommandKey, new StringBuilder());
					}

                    HasExecutingProcess = true;
                    DispatchNewStateKey();

                    try
                    {
						if (terminalCommand.BeginWith is not null)
                            await terminalCommand.BeginWith.Invoke();

                        if (terminalCommand.FormattedCommand.TargetFileName == "netcoredbg")
                        {
                            // TODO: Delete this code block I'm hackily trying to figure out...
                            // ...how to provide input to netcoredbg, instead of it immediately quitting.

                            // I'm going to use the visual studio debugger to set this programPid at runtime
                            var programPid = "";

                            command.WithStandardInputPipe(PipeSource.FromString($"attach {programPid}\n"));
                        }

                        await command.Observe(_commandCancellationTokenSource.Token)
                            .ForEachAsync(cmdEvent =>
                            {
								var output = (string?)null;

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
										_standardOutBuilderMap[terminalCommandKey].AppendLine(output);
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
	            stringBuilder.Clear();
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