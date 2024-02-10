using CliWrap;
using CliWrap.EventStream;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Reactive.Linq;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals.Test;

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
                            _cliWrapIntegratedTerminal.AddStandardOut($"{output}{Environment.NewLine}");
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
}