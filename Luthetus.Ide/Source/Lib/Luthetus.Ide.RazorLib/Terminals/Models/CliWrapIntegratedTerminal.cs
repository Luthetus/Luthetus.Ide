using CliWrap;
using CliWrap.EventStream;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Text;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class CliWrapIntegratedTerminal : IntegratedTerminal
{
    private readonly List<Std> _stdList = new();
    /// <summary>
    /// https://github.com/Tyrrrz/CliWrap/issues/191
    /// (Topic is accepting user input with CliWrap)
    /// </summary>
    private readonly SemaphoreSlim _stdInputSemaphore = new SemaphoreSlim(0, 1);
    private readonly StringBuilder _stdInputBuffer = new StringBuilder();

    public CliWrapIntegratedTerminal(string initialWorkingDirectory, IEnvironmentProvider environmentProvider)
        : base(initialWorkingDirectory, environmentProvider)
    {
        _stdList.Add(new StdQuiescent(this));
    }

    public PipeSource? StdInPipeSource { get; private set; }
    public override ImmutableArray<Std> StdList => _stdList.ToImmutableArray();

    public void AddStdOut(string content, StdOutKind stdOutKind)
    {
        var existingStd = _stdList.LastOrDefault();

        if (existingStd is not null &&
            existingStd is StdOut existingStdOut &&
            existingStdOut.StdOutKind == stdOutKind)
        {
            existingStdOut.Content += content;
        }
        else
        {
            _stdList.Add(new StdOut(this, content, stdOutKind));
        }
    }

    public void AddStdInRequest()
    {
        _stdList.Add(new StdInRequest(this));
    }

    public override async Task StartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

                if (TaskQueue.TryDequeue(out var func))
                {
                    await func.Invoke();
                    _stdList.Add(new StdQuiescent(this));
                    InvokeStateChanged();
                }
            }
        }
        catch (TaskCanceledException)
        {
            // eat this exception?
        }

        await StopAsync();
    }

    public override Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public override Task HandleStdInputOnKeyDown(
        KeyboardEventArgs keyboardEventArgs,
        StdInRequest stdInRequest,
        string capturedValue)
    {
        if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            stdInRequest.IsCompleted = true;
            stdInRequest.Value = capturedValue;
            InvokeStateChanged();

            _stdInputBuffer.Clear();
            _stdInputBuffer.AppendLine(capturedValue);
            _stdInputSemaphore.Release();
        }

        return Task.CompletedTask;
    }

    public override Task HandleStdQuiescentOnKeyDown(
        KeyboardEventArgs keyboardEventArgs,
        StdQuiescent stdQuiescent,
        string capturedTargetFilePath,
        string capturedArguments)
    {
        if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            stdQuiescent.IsCompleted = true;
            stdQuiescent.TargetFilePath = capturedTargetFilePath;
            stdQuiescent.Arguments = capturedArguments;

            TargetFilePath = capturedTargetFilePath;
            Arguments = capturedArguments;

            InvokeStateChanged();

            TaskQueue.Enqueue(async () =>
            {
                StdInPipeSource = PipeSource.Create(async (destination, cancellationToken) =>
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    _ = Task.Run(() =>
                    {
                        // The UI element for stdin should render,
                        // accept input, and upon 'Enter' key,
                        // release the '_stdInputSemaphore'
                        AddStdInRequest();
                        InvokeStateChanged();
                        return Task.CompletedTask;
                    });

                    await _stdInputSemaphore.WaitAsync(cancellationToken);
                    var data = Encoding.UTF8.GetBytes(_stdInputBuffer.ToString());
                    await destination.WriteAsync(data, 0, data.Length, cancellationToken);
                });

                var command = Cli
                    .Wrap(TargetFilePath)
                    .WithArguments(Arguments)
                    .WithStandardInputPipe(StdInPipeSource);

                await command.Observe()
                    .ForEachAsync(cmdEvent =>
                    {
                        var output = (string?)null;
                        var outputKind = StdOutKind.None;

                        switch (cmdEvent)
                        {
                            case StartedCommandEvent started:
                                output = $"(PID:{started.ProcessId})";
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
                            AddStdOut(
                                $"{output}{Environment.NewLine}",
                                outputKind);

                            InvokeStateChanged();
                        }
                    });
            });
        }

        return Task.CompletedTask;
    }
}
