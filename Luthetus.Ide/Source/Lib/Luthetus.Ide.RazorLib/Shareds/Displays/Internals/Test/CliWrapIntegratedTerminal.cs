using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals.Test;

public class CliWrapIntegratedTerminal : IntegratedTerminal
{
    private readonly List<Std> _stdList = new();

    public CliWrapIntegratedTerminal(string initialWorkingDirectory, IEnvironmentProvider environmentProvider)
        : base(initialWorkingDirectory, environmentProvider)
    {
        _stdList.Add(new StdIn(this));
    }

    public void AddStandardOut(string content, StdOutKind stdOutKind)
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
        }

        return builder;
    }

    public override async Task StopAsync(CancellationToken cancellationToken = default)
    {
    }
}
