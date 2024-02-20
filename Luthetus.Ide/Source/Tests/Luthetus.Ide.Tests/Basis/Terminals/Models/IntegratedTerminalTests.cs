using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Concurrent;

namespace Luthetus.Ide.Tests.Basis.Terminals.Models;

public abstract class IntegratedTerminalTests
{
    [Fact]
    public void Constructor()
    {
        //public IntegratedTerminal(string initialWorkingDirectory, IEnvironmentProvider environmentProvider)
    }

    [Fact]
    public void WorkingDirectory()
    {
        //public string  { get; }
    }

    [Fact]
    public void TargetFilePath()
    {
        //public string  { get; set; } = "netcoredbg";
    }

    [Fact]
    public void Arguments()
    {
        //public string  { get; set; } = "--interpreter=cli -- dotnet C:\\Users\\hunte\\Repos\\Demos\\BlazorApp4NetCoreDbg\\ConsoleApp1\\bin\\Debug\\net8.0\\ConsoleApp1.dll";
    }

    [Fact]
    public void EnvironmentProvider()
    {
        //public IEnvironmentProvider  { get; }
    }

    [Fact]
    public void TaskQueue()
    {
        //public ConcurrentQueue<Func<Task>>  { get; } = new();
    }

    [Fact]
    public void StateChanged()
    {
        //public event Action? ;
    }

    [Fact]
    public void HandleStdInputOnKeyDown()
    {
        //public abstract Task (KeyboardEventArgs keyboardEventArgs, StdInRequest stdInRequest, string capturedValue);
    }

    [Fact]
    public void HandleStdQuiescentOnKeyDown()
    {
        //public abstract Task (KeyboardEventArgs keyboardEventArgs, StdQuiescent stdQuiescent, string capturedTargetFilePath, string capturedArguments);
    }

    [Fact]
    public void StartAsync()
    {
        //public abstract Task (CancellationToken cancellationToken = default);
    }

    [Fact]
    public void GetRenderTreeBuilder()
    {
        //public abstract RenderTreeBuilder (RenderTreeBuilder builder, ref int sequence);
    }

    [Fact]
    public void StopAsync()
    {
        //public abstract Task (CancellationToken cancellationToken = default);
    }

    [Fact]
    public void InvokeStateChanged()
    {
        //StateChanged?.Invoke();
    }
}
