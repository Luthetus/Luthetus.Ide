using Luthetus.Common.RazorLib.FileSystems.Models;
using System.Collections.Concurrent;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals.Test;

public abstract class IntegratedTerminal
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
