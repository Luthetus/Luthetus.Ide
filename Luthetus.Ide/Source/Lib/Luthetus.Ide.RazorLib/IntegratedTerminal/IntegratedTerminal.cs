using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Concurrent;

namespace Luthetus.Ide.RazorLib.IntegratedTerminal;

public abstract class IntegratedTerminal
{
    public IntegratedTerminal(string initialWorkingDirectory, IEnvironmentProvider environmentProvider)
    {
        WorkingDirectory = initialWorkingDirectory;
        EnvironmentProvider = environmentProvider;
    }

    public string WorkingDirectory { get; }
    public string TargetFilePath { get; set; } = "\\Users\\hunte\\Repos\\Demos\\TestingCliWrap\\a.out";
    public string Arguments { get; set; } = string.Empty;
    public IEnvironmentProvider EnvironmentProvider { get; }
    public ConcurrentQueue<Func<Task>> TaskQueue { get; } = new();
    
    public event Action? StateChanged;
    
    public abstract Task HandleStdInputOnKeyDown(KeyboardEventArgs keyboardEventArgs, StdInRequest stdInRequest, string capturedValue);
    public abstract Task HandleStdQuiescentOnKeyDown(KeyboardEventArgs keyboardEventArgs, StdQuiescent stdQuiescent, string capturedTargetFilePath, string capturedArguments);
    public abstract Task StartAsync(CancellationToken cancellationToken = default);
    public abstract RenderTreeBuilder GetRenderTreeBuilder(RenderTreeBuilder builder, ref int sequence);
    public abstract Task StopAsync(CancellationToken cancellationToken = default);

    protected virtual void InvokeStateChanged()
    {
        StateChanged?.Invoke();
    }
}
