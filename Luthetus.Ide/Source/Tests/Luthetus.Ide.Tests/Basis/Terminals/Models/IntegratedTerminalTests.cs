using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Concurrent;

namespace Luthetus.Ide.Tests.Basis.Terminals.Models;

public abstract class IntegratedTerminalTests
{
    [Fact]
    public void Aaa()
    {
        //public IntegratedTerminal(string initialWorkingDirectory, IEnvironmentProvider environmentProvider)
    }

    [Fact]
    public void Aaa()
    {
        //public string WorkingDirectory { get; }
    }

    [Fact]
    public void Aaa()
    {
        //public string TargetFilePath { get; set; } = "netcoredbg";
    }

    [Fact]
    public void Aaa()
    {
        //public string Arguments { get; set; } = "--interpreter=cli -- dotnet C:\\Users\\hunte\\Repos\\Demos\\BlazorApp4NetCoreDbg\\ConsoleApp1\\bin\\Debug\\net8.0\\ConsoleApp1.dll";
    }

    [Fact]
    public void Aaa()
    {
        //public IEnvironmentProvider EnvironmentProvider { get; }
    }

    [Fact]
    public void Aaa()
    {
        //public ConcurrentQueue<Func<Task>> TaskQueue { get; } = new();
    }

    [Fact]
    public void Aaa()
    {
        //public event Action? StateChanged;
    }

    [Fact]
    public void Aaa()
    {
        //public abstract Task HandleStdInputOnKeyDown(KeyboardEventArgs keyboardEventArgs, StdInRequest stdInRequest, string capturedValue);
    }

    [Fact]
    public void Aaa()
    {
        //public abstract Task HandleStdQuiescentOnKeyDown(KeyboardEventArgs keyboardEventArgs, StdQuiescent stdQuiescent, string capturedTargetFilePath, string capturedArguments);
    }

    [Fact]
    public void Aaa()
    {
        //public abstract Task StartAsync(CancellationToken cancellationToken = default);
    }

    [Fact]
    public void Aaa()
    {
        //public abstract RenderTreeBuilder GetRenderTreeBuilder(RenderTreeBuilder builder, ref int sequence);
    }

    [Fact]
    public void Aaa()
    {
        //public abstract Task StopAsync(CancellationToken cancellationToken = default);
    }

    protected virtual void InvokeStateChanged()
    {
        StateChanged?.Invoke();
    }
}
