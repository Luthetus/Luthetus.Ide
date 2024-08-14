using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Exceptions;

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
        LuthetusPurposeKind luthetusPurposeKind,
        IBackgroundTaskService backgroundTaskService)
    {
        LuthetusHostingKind = luthetusHostingKind;
        LuthetusPurposeKind = luthetusPurposeKind;
        BackgroundTaskService = backgroundTaskService;
    }

    public LuthetusHostingKind LuthetusHostingKind { get; init; }
    public LuthetusPurposeKind LuthetusPurposeKind { get; init; }
    public IBackgroundTaskService BackgroundTaskService { get; init; }
    /// <summary>
    /// If the main window hasn't been initialized yet, 0 is returned.
    /// Whether 0 returns at other points is uncertain.
    /// 
    /// This also returns 0 if the host isn't Photino (i.e.: ServerSide Blazor or Wasm Blazor)
    /// </summary>
    public Func<uint> GetMainWindowScreenDpiFunc { get; set; } = () => 0;

    public void StartBackgroundTaskWorkers(IServiceProvider serviceProvider)
    {
    	if (LuthetusHostingKind == LuthetusHostingKind.ServerSide)
    	{
    		throw new LuthetusFatalException(
    			$"The '{nameof(LuthetusHostingKind)}' is '{nameof(LuthetusHostingKind.ServerSide)}';" +
    			$"Therefore, do not invoke '{nameof(LuthetusHostingInformation)}.{nameof(StartBackgroundTaskWorkers)}(...)'," +
    			$"because the background task workers will be started instead with" +
    			" 'services.AddHostedService(sp => sp.GetRequiredService<ContinuousBackgroundTaskWorker>());' and etc...," +
    			"within the '{nameof(AddLuthetusCommonServices)}' method, automatically.");
    	}
    
		var continuousCtsUp = new CancellationTokenSource();
        var continuousCtsDown = new CancellationTokenSource();
        var continuousBtw = serviceProvider.GetRequiredService<ContinuousBackgroundTaskWorker>();
        continuousBtw.StartAsyncTask = continuousBtw.StartAsync(continuousCtsUp.Token);
        Task continuousTaskDown;

		CancellationTokenSource? blockingCtsUp = null;
		CancellationTokenSource? blockingCtsDown = null;
		BlockingBackgroundTaskWorker? blockingBtw = null;
		Task? blockingTaskDown = null;
		if (LuthetusPurposeKind == LuthetusPurposeKind.Ide)
		{
			blockingCtsUp = new CancellationTokenSource();
	        blockingCtsDown = new CancellationTokenSource();
	        blockingBtw = serviceProvider.GetRequiredService<BlockingBackgroundTaskWorker>();
	        blockingBtw.StartAsyncTask = blockingBtw.StartAsync(blockingCtsUp.Token);
		}

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            continuousCtsUp.Cancel();
            continuousTaskDown = continuousBtw.StopAsync(continuousCtsDown.Token);
            
            if (blockingCtsUp is not null)
            	blockingCtsUp.Cancel();
            
            if (blockingBtw is not null)
        		blockingTaskDown = blockingBtw.StopAsync(blockingCtsDown.Token);
        };

        AppDomain.CurrentDomain.ProcessExit += (sender, error) =>
        {
            continuousCtsUp.Cancel();
            continuousCtsDown.Cancel();
            
            if (blockingCtsUp is not null)
            	blockingCtsUp.Cancel();

            if (blockingCtsDown is not null)
	            blockingCtsDown.Cancel();
        };
    }
}