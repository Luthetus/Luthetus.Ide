using CliWrap;
using CliWrap.EventStream;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Text;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals;

public partial class TestIntegratedTerminal : ComponentBase, IDisposable
{
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    private readonly object TerminalLock = new();

    private CliWrapIntegratedTerminal _cliWrapIntegratedTerminal = null!;
    private CancellationTokenSource _terminalCancellationTokenSource = new();

    private string _workingDirectory = string.Empty;
    private string _shellAbsolutePath = string.Empty;
    private string _stdOut = string.Empty;
    private string _stdErr = string.Empty;
    private string _stdIn = string.Empty;

    private string _output = string.Empty;
    private string _targetFilePath = "netcoredbg";
    private string _arguments = "--interpreter=cli -- dotnet \\Users\\hunte\\Repos\\Demos\\BlazorApp4NetCoreDbg\\BlazorApp4NetCoreDbg\\bin\\Debug\\net6.0\\BlazorApp4NetCoreDbg.dll";

    private Task _terminalTask = Task.CompletedTask;

    protected override void OnInitialized()
    {
        _workingDirectory = EnvironmentProvider.HomeDirectoryAbsolutePath.Value;
        _cliWrapIntegratedTerminal = new(_workingDirectory, EnvironmentProvider);

        base.OnInitialized();
    }

    protected void StdInHandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            var command = Cli
                .Wrap(_targetFilePath)
                .WithArguments(_arguments);

            _cliWrapIntegratedTerminal.TaskQueue.Enqueue(async () =>
            {
                await command.Observe()
                    .ForEachAsync(async cmdEvent =>
                    {
                        var output = (string?)null;

                        switch (cmdEvent)
                        {
                            case StartedCommandEvent started:
                                output = $"> {_workingDirectory} (PID:{started.ProcessId}) {command.ToString()}";
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
                            _stdOut += $"{output}{Environment.NewLine}";
                            await InvokeAsync(StateHasChanged);
                        }
                    });
            });
        }
    }

    private void StartTerminalOnClick()
    {
        if (_terminalTask.IsCompleted)
        {
            lock (TerminalLock)
            {
                if (_terminalTask.IsCompleted)
                {
                    _terminalCancellationTokenSource.Cancel();
                    _terminalCancellationTokenSource = new();

                    var cancellationToken = _terminalCancellationTokenSource.Token;

                    _terminalTask = Task.Run(async () => 
                    {
                        await _cliWrapIntegratedTerminal.StartAsync(cancellationToken);
                    });

                    _terminalTask.ContinueWith(async _ =>
                    {
                        // Stop UI rendering the spinner
                        await InvokeAsync(StateHasChanged);
                    });
                }
            }
        }
    }

    private void CancelTerminalOnClick()
    {
        _terminalCancellationTokenSource.Cancel();
        _terminalCancellationTokenSource = new();
    }

    public void Dispose()
    {
        _terminalCancellationTokenSource.Cancel();
    }

    private abstract class IntegratedTerminal
    {
        public IntegratedTerminal(string initialWorkingDirectory, IEnvironmentProvider environmentProvider)
        {
            WorkingDirectory = initialWorkingDirectory;
            EnvironmentProvider = environmentProvider;
        }

        public string WorkingDirectory { get; }
        public IEnvironmentProvider EnvironmentProvider { get; }
        public ConcurrentQueue<Func<Task>> TaskQueue { get; } = new();

        public abstract Task StartAsync(CancellationToken cancellationToken = default);
        public abstract string Render();
        public abstract Task StopAsync(CancellationToken cancellationToken = default);
    }

    private class CliWrapIntegratedTerminal : IntegratedTerminal
    {
        private readonly List<Std> _stdList = new();

        public CliWrapIntegratedTerminal(string initialWorkingDirectory, IEnvironmentProvider environmentProvider)
            : base(initialWorkingDirectory, environmentProvider)
        {
            _stdList.Add(new StdIn(this));
        }

        public override async Task StartAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

                    if (TaskQueue.TryDequeue(out var func))
                        await func.Invoke();
                }
            }
            catch (TaskCanceledException)
            {
                // eat this exception?
            }

            await StopAsync();
        }

        public override string Render()
        {
            var outputBuilder = new StringBuilder();
            
            foreach (var std in _stdList)
            {
                std.Render(outputBuilder);
            }

            return outputBuilder.ToString();
        }

        public override async Task StopAsync(CancellationToken cancellationToken = default)
        {
        }
    }

    private abstract class Std
    {
        protected readonly IntegratedTerminal _integratedTerminal;

        public Std(IntegratedTerminal integratedTerminal)
        {
            _integratedTerminal = integratedTerminal;
        }

        public abstract void Render(StringBuilder stringBuilder);
    }

    private class StdOut : Std
    {
        public StdOut(IntegratedTerminal integratedTerminal)
            : base(integratedTerminal)
        {
        }

        public override void Render(StringBuilder stringBuilder)
        { 
        }
    }

    private class StdErr : Std
    {
        public StdErr(IntegratedTerminal integratedTerminal)
            : base(integratedTerminal)
        {
        }

        public override void Render(StringBuilder stringBuilder)
        {
        }
    }

    private class StdIn : Std
    {
        public StdIn(IntegratedTerminal integratedTerminal)
            : base(integratedTerminal)
        {
        }

        public override void Render(StringBuilder stringBuilder)
        {
            var workingDirectoryAbsolutePath = _integratedTerminal.EnvironmentProvider.AbsolutePathFactory(
                _integratedTerminal.WorkingDirectory,
                true);

            var showWorkingDirectory = workingDirectoryAbsolutePath.NameNoExtension;

            var parentDirectory = workingDirectoryAbsolutePath.ParentDirectory;

            if (parentDirectory is not null)
                showWorkingDirectory = parentDirectory.Value + showWorkingDirectory;

            stringBuilder.Append($"{showWorkingDirectory}>");
        }
    }
}