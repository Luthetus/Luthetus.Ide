using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public abstract class IntegratedTerminal
{
    public IntegratedTerminal(string initialWorkingDirectory, IEnvironmentProvider environmentProvider)
    {
        WorkingDirectory = initialWorkingDirectory;
        EnvironmentProvider = environmentProvider;
    }

    public string WorkingDirectory { get; }

    public string TargetFilePath { get; set; } = "netcoredbg";
    public string Arguments { get; set; } = "--interpreter=cli -- dotnet C:\\Users\\hunte\\Repos\\Demos\\BlazorApp4NetCoreDbg\\ConsoleApp1\\bin\\Debug\\net8.0\\ConsoleApp1.dll";
    public IEnvironmentProvider EnvironmentProvider { get; }
    public ConcurrentQueue<Func<Task>> TaskQueue { get; } = new();
    
    public abstract ImmutableArray<Std> StdList { get; }

    public event Action? StateChanged;
    
    public abstract Task HandleStdInputOnKeyDown(KeyboardEventArgs keyboardEventArgs, StdInRequest stdInRequest, string capturedValue);
    public abstract Task HandleStdQuiescentOnKeyDown(KeyboardEventArgs keyboardEventArgs, StdQuiescent stdQuiescent, string capturedTargetFilePath, string capturedArguments);
    public abstract Task StartAsync(CancellationToken cancellationToken = default);
    public abstract Task StopAsync(CancellationToken cancellationToken = default);

    protected virtual void InvokeStateChanged()
    {
        StateChanged?.Invoke();
    }
}
