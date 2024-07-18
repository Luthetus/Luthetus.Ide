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
/// <remarks>
/// This class is an exception to the naming convention, "don't use the word 'Luthetus' in class names".
/// 
/// Reason for this exception: when one first starts interacting with this project,
/// 	this type might be one of the first types they interact with. So, the redundancy of namespace
/// 	and type containing 'Luthetus' feels reasonable here.
/// </remarks>
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