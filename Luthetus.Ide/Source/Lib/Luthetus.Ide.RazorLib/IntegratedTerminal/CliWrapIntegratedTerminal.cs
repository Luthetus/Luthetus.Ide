﻿using CliWrap;
using CliWrap.EventStream;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System.Reactive.Linq;
using System.Text;

namespace Luthetus.Ide.RazorLib.IntegratedTerminal;

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
        _stdList.Add(new StdIn(this));
    }

    private string _workingDirectory = string.Empty;
    private string _targetFilePath = "\\Users\\hunte\\Repos\\Demos\\TestingCliWrap\\a.out";//"netcoredbg";
    private string _arguments = string.Empty;//"--interpreter=cli -- dotnet \\Users\\hunte\\Repos\\Demos\\BlazorApp4NetCoreDbg\\BlazorApp4NetCoreDbg\\bin\\Debug\\net6.0\\BlazorApp4NetCoreDbg.dll";

    public PipeSource? StdInPipeSource { get; private set; }

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
        // _stdList.Add(new StdOut(this, content, stdOutKind));
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

    public override RenderTreeBuilder GetRenderTreeBuilder(RenderTreeBuilder builder, ref int sequence)
    {
        foreach (var std in _stdList)
        {
            std.GetRenderTreeBuilder(builder, ref sequence);
            builder.OpenElement(sequence++, "hr");
            builder.CloseElement();
        }

        return builder;
    }

    public override Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public override Task HandleStdInputOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (ShowHtmlElementStdInput && keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
            ShowHtmlElementStdInput = false;
            InvokeStateChanged();
            _stdInputBuffer.Clear();
            _stdInputBuffer.AppendLine(HtmlElementBindStdInput);
            _stdInputSemaphore.Release();
        }

        return Task.CompletedTask;
    }

    public override Task HandleOnKeyDown(KeyboardEventArgs keyboardEventArgs)
    {
        if (keyboardEventArgs.Code == KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE)
        {
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
                        ShowHtmlElementStdInput = true;
                        InvokeStateChanged();
                        return Task.CompletedTask;
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
                    .ForEachAsync(cmdEvent =>
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