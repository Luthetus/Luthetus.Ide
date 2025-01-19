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
    	/*
    	if (LuthetusHostingKind == LuthetusHostingKind.ServerSide)
    	{
    		throw new LuthetusFatalException(
    			$"The '{nameof(LuthetusHostingKind)}' is '{nameof(LuthetusHostingKind.ServerSide)}';" +
    			$"Therefore, do not invoke '{nameof(LuthetusHostingInformation)}.{nameof(StartBackgroundTaskWorkers)}(...)'," +
    			$"because the background task workers will be started instead with" +
    			" 'services.AddHostedService(sp => sp.GetRequiredService<ContinuousBackgroundTaskWorker>());' and etc...," +
    			"within the '{nameof(AddLuthetusCommonServices)}' method, automatically.");
    	}
    	
    	var backgroundTaskService = serviceProvider.GetRequiredService<IBackgroundTaskService>();
    
		var continuousCtsUp = new CancellationTokenSource();
        var continuousCtsDown = new CancellationTokenSource();
        var continuousBtw = backgroundTaskService.ContinuousTaskWorker;
        continuousBtw.StartAsyncTask = continuousBtw.StartAsync(continuousCtsUp.Token);
        Task continuousTaskDown;

		CancellationTokenSource? indefiniteCtsUp = null;
		CancellationTokenSource? indefiniteCtsDown = null;
		BackgroundTaskWorker? indefiniteBtw = null;
		Task? indefiniteTaskDown = null;
		if (LuthetusPurposeKind == LuthetusPurposeKind.Ide)
		{
			indefiniteCtsUp = new CancellationTokenSource();
	        indefiniteCtsDown = new CancellationTokenSource();
	        indefiniteBtw = backgroundTaskService.IndefiniteTaskWorker;
	        indefiniteBtw.StartAsyncTask = indefiniteBtw.StartAsync(indefiniteCtsUp.Token);
		}

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            continuousCtsUp.Cancel();
            continuousTaskDown = continuousBtw.StopAsync(continuousCtsDown.Token);
            
            if (indefiniteCtsUp is not null)
            	indefiniteCtsUp.Cancel();
            
            if (indefiniteBtw is not null)
        		indefiniteTaskDown = indefiniteBtw.StopAsync(indefiniteCtsDown.Token);
        };

        AppDomain.CurrentDomain.ProcessExit += (sender, error) =>
        {
            continuousCtsUp.Cancel();
            continuousCtsDown.Cancel();
            
            if (indefiniteCtsUp is not null)
            	indefiniteCtsUp.Cancel();

            if (indefiniteCtsDown is not null)
	            indefiniteCtsDown.Cancel();
        };
        */
    }
}