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
    private string _targetFilePath = "\\Users\\hunte\\Repos\\Demos\\TestingCliWrap\\a.out";//"netcoredbg";
    private string _arguments = string.Empty;//"--interpreter=cli -- dotnet \\Users\\hunte\\Repos\\Demos\\BlazorApp4NetCoreDbg\\BlazorApp4NetCoreDbg\\bin\\Debug\\net6.0\\BlazorApp4NetCoreDbg.dll";

    private Task _terminalTask = Task.CompletedTask;

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

    public string StdIn
    {
        get => _stdIn;
        set => _stdIn = value;
    }
    public PipeSource? StdInPipeSource { get; private set; }

    protected override void OnInitialized()
    {
        _workingDirectory = EnvironmentProvider.HomeDirectoryAbsolutePath.Value;
        _cliWrapIntegratedTerminal = new(_workingDirectory, EnvironmentProvider);

        StdInStream = new MemoryStream(StdInBuffer);

        base.OnInitialized();
    }

    protected void HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            StdInPipeSource = PipeSource.FromStream(StdInStream, true);

            var command = Cli
                .Wrap(_targetFilePath)
                .WithArguments(_arguments)
                .WithStandardInputPipe(StdInPipeSource);

            _cliWrapIntegratedTerminal.TaskQueue.Enqueue(async () =>
            {
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

                                // https://stackoverflow.com/questions/5769494/reusing-memory-streams
                                //
                                // Reset the stream so you can re-use it
                                StdInStream.Position = 0; // Not actually needed, SetLength(0) will reset the Position anyway
                                StdInStream.SetLength(0);

                                var stdInWriter = new StreamWriter(StdInStream);
                                stdInWriter.Write($"attach {started.ProcessId}\n");
                                stdInWriter.Flush();

                                // https://stackoverflow.com/questions/78181/how-do-you-get-a-string-from-a-memorystream
                                //
                                // The StreamReader will read from the current 
                                // position of the MemoryStream which is currently 
                                // set at the end of the string we just wrote to it. 
                                // We need to set the position to 0 in order to read 
                                // from the beginning.
                                StdInStream.Position = 0;

                                // Sanity checking that StreamReader returns what I want.
                                {
                                    //var stdInReader = new StreamReader(StdInStream);
                                    //var line = stdInReader.ReadLine();
                                }
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
        StdInStream.Dispose();
    }
}