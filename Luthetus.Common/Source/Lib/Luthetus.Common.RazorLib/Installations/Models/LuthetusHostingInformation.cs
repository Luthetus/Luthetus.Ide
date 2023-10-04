using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Installations.Models;

/// <summary>
/// One use case for <see cref="LuthetusHostingInformation"/> would be service registration.<br/><br/>
/// If one uses <see cref="LuthetusHostingKind.ServerSide"/>, then 
/// services.AddHostedService&lt;TService&gt;(...); will be invoked.<br/><br/>
/// Whereas, if one uses <see cref="LuthetusHostingKind.Wasm"/> then 
/// services.AddSingleton&lt;TService&gt;(...); will be used.
/// Then after the initial render, a Task will be 'fire and forget' invoked to start the service.
/// </summary>
public record LuthetusHostingInformation
{
    public LuthetusHostingInformation(
        LuthetusHostingKind luthetusHostingKind,
        IBackgroundTaskService backgroundTaskService)
    {
        LuthetusHostingKind = luthetusHostingKind;
        BackgroundTaskService = backgroundTaskService;
    }

    public LuthetusHostingKind LuthetusHostingKind { get; init; }
    public IBackgroundTaskService BackgroundTaskService { get; init; }
}