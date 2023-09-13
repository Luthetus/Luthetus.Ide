using CliWrap;
using CliWrap.EventStream;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Ide.RazorLib.StateCase;
using Luthetus.TextEditor.RazorLib.Model;
using Luthetus.TextEditor.RazorLib.ViewModel.InternalClasses;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Text;

namespace Luthetus.Ide.RazorLib.TerminalCase;

public class TerminalSession
{
    private readonly IDispatcher _dispatcher;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly ILuthetusCommonComponentRenderers _luthetusCommonComponentRenderers;
    private readonly List<TerminalCommand> _terminalCommandsHistory = new();
    private CancellationTokenSource _commandCancellationTokenSource = new();

    private readonly ConcurrentQueue<TerminalCommand> _terminalCommandsConcurrentQueue = new();

    /// <summary>
    /// TODO: Prove that standard error is correctly being redirected to standard out
    /// </summary>
    private readonly Dictionary<TerminalCommandKey, StringBuilder> _standardOutBuilderMap = new();

    public TerminalSession(
        string? workingDirectoryAbsolutePathString,
        IDispatcher dispatcher,
        IFileSystemProvider fileSystemProvider,
        IBackgroundTaskService backgroundTaskService,
        ILuthetusCommonComponentRenderers luthetusCommonComponentRenderers)
    {
        _dispatcher = dispatcher;
        _fileSystemProvider = fileSystemProvider;
        _backgroundTaskService = backgroundTaskService;
        _luthetusCommonComponentRenderers = luthetusCommonComponentRenderers;
        WorkingDirectoryAbsolutePathString = workingDirectoryAbsolutePathString;
    }

    public TerminalSessionKey TerminalSessionKey { get; init; } =
        TerminalSessionKey.NewKey();

    public TextEditorModelKey TextEditorModelKey => new(TerminalSessionKey.Guid);
    public TextEditorViewModelKey TextEditorViewModelKey => new(TerminalSessionKey.Guid);

    public string? WorkingDirectoryAbsolutePathString { get; private set; }

    public TerminalCommand? ActiveTerminalCommand { get; private set; }

    public ImmutableArray<TerminalCommand> TerminalCommandsHistory => _terminalCommandsHistory.ToImmutableArray();

    // NOTE: the following did not work => _process?.HasExited ?? false;
    public bool HasExecutingProcess { get; private set; }

    public string ReadStandardOut()
    {
        return string
            .Join(string.Empty, _standardOutBuilderMap
                .Select(x => x.Value.ToString())
                .ToArray());
    }

    public string? ReadStandardOut(TerminalCommandKey terminalCommandKey)
    {
        if (_standardOutBuilderMap
            .TryGetValue(terminalCommandKey, out var output))
        {
            return output.ToString();
        }

        return null;
    }

    public Task EnqueueCommandAsync(TerminalCommand terminalCommand)
    {
        _backgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), TerminalBackgroundTaskWorker.Queue.Key,
            "Enqueue Command",
            async () =>
            {
                if (terminalCommand.ChangeWorkingDirectoryTo is not null)
                    WorkingDirectoryAbsolutePathString = terminalCommand.ChangeWorkingDirectoryTo;

                if (terminalCommand.FormattedCommand.TargetFileName == "cd" &&
                    terminalCommand.FormattedCommand.Arguments.Any())
                {
                    // TODO: Don't keep this logic as it is hacky. I'm trying to set myself up to be able to run "gcc" to compile ".c" files. Then I can work on adding symbol related logic like "go to definition" or etc.
                    WorkingDirectoryAbsolutePathString = terminalCommand.FormattedCommand.Arguments.ElementAt(0);
                }

                _terminalCommandsHistory.Add(terminalCommand);
                ActiveTerminalCommand = terminalCommand;

                var command = Cli.Wrap(terminalCommand.FormattedCommand.TargetFileName);

                if (terminalCommand.FormattedCommand.Arguments.Any())
                    command = command.WithArguments(terminalCommand.FormattedCommand.Arguments);

                if (terminalCommand.ChangeWorkingDirectoryTo is not null)
                {
                    command = command
                        .WithWorkingDirectory(terminalCommand.ChangeWorkingDirectoryTo);
                }
                else if (WorkingDirectoryAbsolutePathString is not null)
                {
                    command = command
                        .WithWorkingDirectory(WorkingDirectoryAbsolutePathString);
                }

                // Push-based event stream
                {
                    var terminalCommandKey = terminalCommand.TerminalCommandKey;

                    _standardOutBuilderMap.TryAdd(
                        terminalCommand.TerminalCommandKey,
                        new StringBuilder());

                    HasExecutingProcess = true;
                    DispatchNewStateKey();

                    try
                    {
                        await command
                            .Observe(_commandCancellationTokenSource.Token)
                            .ForEachAsync(cmdEvent =>
                            {
                                switch (cmdEvent)
                                {
                                    case StartedCommandEvent started:
                                        _standardOutBuilderMap[terminalCommandKey].AppendLine(
                                            $"> {WorkingDirectoryAbsolutePathString} (PID:{started.ProcessId}) {terminalCommand.FormattedCommand.Value}");

                                        DispatchNewStateKey();
                                        break;
                                    case StandardOutputCommandEvent stdOut:
                                        _standardOutBuilderMap[terminalCommandKey].AppendLine(
                                            $"{stdOut.Text}");

                                        DispatchNewStateKey();
                                        break;
                                    case StandardErrorCommandEvent stdErr:
                                        _standardOutBuilderMap[terminalCommandKey].AppendLine(
                                            $"Err> {stdErr.Text}");

                                        DispatchNewStateKey();
                                        break;
                                    case ExitedCommandEvent exited:
                                        _standardOutBuilderMap[terminalCommandKey].AppendLine(
                                            $"Process exited; Code: {exited.ExitCode}");

                                        DispatchNewStateKey();
                                        break;
                                }
                            });
                    }
                    catch (Exception e)
                    {
                        NotificationHelper.DispatchError("Terminal Exception", e.ToString(), _luthetusCommonComponentRenderers, _dispatcher);
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
        // TODO: Rewrite this - see contiguous comment block
        //
        // This is awkward but concurrency exceptions I believe might occur
        // otherwise given the current way the code is written (2022-02-11)
        //
        // If one tries to write to standard out dictionary they need a key value entry
        // to exist or they add one
        // 
        // If one sees a key value entry exists they can use the existing StringBuilder
        // but I am tempted to write _standardOutBuilderMap.Clear() thereby
        // clearing all the key value pairs as they write to the StringBuilder.
        foreach (var stringBuilder in _standardOutBuilderMap.Values)
        {
            stringBuilder.Clear();
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
        _dispatcher.Dispatch(new TerminalSessionWasModifiedRegistryReducer.SetTerminalSessionStateKeyAction(
            TerminalSessionKey,
            StateKey.NewKey()));
    }
}