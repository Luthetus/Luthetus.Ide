using CliWrap;
using CliWrap.EventStream;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Reactive.Linq;
using System.Text;
using System.Threading;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals.Test;

public partial class TestIntegratedTerminal : ComponentBase, IDisposable
{
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    private readonly object TerminalLock = new();
    /// <summary>
    /// https://github.com/Tyrrrz/CliWrap/issues/191
    /// (Topic is accepting user input with CliWrap)
    /// </summary>
    private SemaphoreSlim _stdInputSemaphore = new SemaphoreSlim(0, 1);
    private StringBuilder _stdInputBuffer = new StringBuilder();

    private CliWrapIntegratedTerminal _cliWrapIntegratedTerminal = null!;
    private CancellationTokenSource _terminalCancellationTokenSource = new();

    private string _workingDirectory = string.Empty;
    private string _targetFilePath = "\\Users\\hunte\\Repos\\Demos\\TestingCliWrap\\a.out";//"netcoredbg";
    private string _arguments = string.Empty;//"--interpreter=cli -- dotnet \\Users\\hunte\\Repos\\Demos\\BlazorApp4NetCoreDbg\\BlazorApp4NetCoreDbg\\bin\\Debug\\net6.0\\BlazorApp4NetCoreDbg.dll";

    private Task _terminalTask = Task.CompletedTask;
    private string _stdInput = string.Empty;
    private bool _showStdInput;

    byte[] StdInBuffer 
    { 
        get;
        set;
    } = new byte[100];

    Stream StdInStream
    {
        get;
        set;
    } = null!;

    public PipeSource? StdInPipeSource { get; private set; }

    protected override void OnInitialized()
    {
        _workingDirectory = EnvironmentProvider.HomeDirectoryAbsolutePath.Value;
        _cliWrapIntegratedTerminal = new(_workingDirectory, EnvironmentProvider);

        StdInStream = new MemoryStream(StdInBuffer);

        base.OnInitialized();
    }

    private async Task HandleStdInputOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            _showStdInput = false;
            await InvokeAsync(StateHasChanged);
            _stdInputBuffer.Clear();
            _stdInputBuffer.AppendLine(_stdInput);
            _stdInputSemaphore.Release();
        }
    }

    private void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            _cliWrapIntegratedTerminal.TaskQueue.Enqueue(async () =>
            {
                StdInPipeSource = PipeSource.Create(async (destination, cancellationToken) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    _ = Task.Run(async () =>
                    {
                        // The UI element for stdin should render,
                        // accept input, and upon 'Enter' key,
                        // release the '_stdInputSemaphore'
                        _showStdInput = true;
                        await InvokeAsync(StateHasChanged);
                    });

                    await _stdInputSemaphore.WaitAsync(cancellationToken);
                    var data = Encoding.UTF8.GetBytes(_stdInputBuffer.ToString());
                    await destination.WriteAsync(data, 0, data.Length, cancellationToken);
                });

                var command = Cli
                    .Wrap(_targetFilePath)
                    .WithArguments(_arguments)
                    .WithStandardInputPipe(StdInPipeSource);

                await command.Observe()
                    .ForEachAsync(async cmdEvent =>
                    {
                        var output = (string?)null;
                        var outputKind = StdOutKind.None;

                        switch (cmdEvent)
                        {
                            case StartedCommandEvent started:
                                output = $"> {_workingDirectory} (PID:{started.ProcessId}) {command.ToString()}";
                                outputKind = StdOutKind.Started;
                                break;
                            case StandardOutputCommandEvent stdOut:
                                output = $"{stdOut.Text}";
                                break;
                            case StandardErrorCommandEvent stdErr:
                                output = $"Err> {stdErr.Text}";
                                outputKind = StdOutKind.Error;
                                break;
                            case ExitedCommandEvent exited:
                                output = $"Process exited; Code: {exited.ExitCode}";
                                outputKind = StdOutKind.Exited;
                                break;
                        }

                        if (output is not null)
                        {
                            _cliWrapIntegratedTerminal.AddStandardOut(
                                $"{output}{Environment.NewLine}",
                                outputKind);

                            
                            _ = Task.Run(async () => await InvokeAsync(StateHasChanged)).ConfigureAwait(false);
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
        StdInStream.Dispose();
    }
}