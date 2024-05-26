namespace Luthetus.TextEditor.Tests.Adhoc;

public partial class AdhocRewrite
{
	[Fact]
	public async Task Can_Initialize_ServerSide()
	{
		var hostingInformation = new LuthetusHostingInformation(
		    LuthetusHostingKind.ServerSide,
		    new BackgroundTaskService());
		
		builder.Services.AddLuthetusWebsiteServices(hostingInformation);
		{
			services.AddLuthetusIdeRazorLibServices(hostingInformation);

	        return services.AddFluxor(options => options.ScanAssemblies(
	            typeof(LuthetusCommonConfig).Assembly,
	            typeof(LuthetusTextEditorConfig).Assembly,
	            typeof(Ide.RazorLib.Installations.Models.ServiceCollectionExtensions).Assembly));
		}

		// Luthetus.Common.RazorLib.Installations.Models.ServiceCollectionExtensions;
		{
			var commonConfig = new LuthetusCommonConfig();

	        if (configure is not null)
	            commonConfig = configure.Invoke(commonConfig);
	
	        hostingInformation.BackgroundTaskService.RegisterQueue(new BackgroundTaskQueue(
	            ContinuousBackgroundTaskWorker.GetQueueKey(),
	            ContinuousBackgroundTaskWorker.QUEUE_DISPLAY_NAME));
	
	        hostingInformation.BackgroundTaskService.RegisterQueue(new BackgroundTaskQueue(
	            BlockingBackgroundTaskWorker.GetQueueKey(),
	            BlockingBackgroundTaskWorker.QUEUE_DISPLAY_NAME));
	
	        services.AddSingleton(sp => new ContinuousBackgroundTaskWorker(
	            sp.GetRequiredService<IBackgroundTaskService>(),
	            sp.GetRequiredService<ILoggerFactory>()));
	
	        services.AddSingleton(sp => new BlockingBackgroundTaskWorker(
	            sp.GetRequiredService<IBackgroundTaskService>(),
	            sp.GetRequiredService<ILoggerFactory>()));
	
	        if (hostingInformation.LuthetusHostingKind == LuthetusHostingKind.ServerSide)
	        {
	            services.AddHostedService(sp => sp.GetRequiredService<ContinuousBackgroundTaskWorker>());
	            services.AddHostedService(sp => sp.GetRequiredService<BlockingBackgroundTaskWorker>());
	        }
	
	        services
	            .AddSingleton(commonConfig)
	            .AddSingleton(hostingInformation)
	            .AddSingleton(hostingInformation.BackgroundTaskService)
	            .AddSingleton<ILuthetusCommonComponentRenderers>(_ => _commonRendererTypes)
	            .AddCommonFactories(hostingInformation, commonConfig)
	            .AddScoped<LuthetusCommonBackgroundTaskApi>();
	
	        return services;

			private static IServiceCollection AddCommonFactories(
		        this IServiceCollection services,
		        LuthetusHostingInformation hostingInformation,
		        LuthetusCommonConfig commonConfig)
		    {
		        services
		            .AddScoped(sp => commonConfig.CommonFactories.ClipboardServiceFactory.Invoke(sp))
		            .AddScoped(sp => commonConfig.CommonFactories.DialogServiceFactory.Invoke(sp))
		            .AddScoped(sp => commonConfig.CommonFactories.NotificationServiceFactory.Invoke(sp))
		            .AddScoped(sp => commonConfig.CommonFactories.DragServiceFactory.Invoke(sp))
		            .AddScoped(sp => commonConfig.CommonFactories.DropdownServiceFactory.Invoke(sp))
		            .AddScoped(sp => commonConfig.CommonFactories.AppOptionsServiceFactory.Invoke(sp))
		            .AddScoped(sp => commonConfig.CommonFactories.StorageServiceFactory.Invoke(sp))
		            .AddScoped(sp => commonConfig.CommonFactories.ThemeServiceFactory.Invoke(sp))
		            .AddScoped(sp => commonConfig.CommonFactories.TreeViewServiceFactory.Invoke(sp));
		
		        if (commonConfig.CommonFactories.EnvironmentProviderFactory is not null &&
		            commonConfig.CommonFactories.FileSystemProviderFactory is not null)
		        {
		            services.AddScoped(sp => commonConfig.CommonFactories.EnvironmentProviderFactory.Invoke(sp));
		            services.AddScoped(sp => commonConfig.CommonFactories.FileSystemProviderFactory.Invoke(sp));
		        }
		        else
		        {
		            switch (hostingInformation.LuthetusHostingKind)
		            {
		                case LuthetusHostingKind.Photino:
		                    services.AddScoped<IEnvironmentProvider, LocalEnvironmentProvider>();
		                    services.AddScoped<IFileSystemProvider, LocalFileSystemProvider>();
		                    break;
		                default:
		                    services.AddScoped<IEnvironmentProvider, InMemoryEnvironmentProvider>();
		                    services.AddScoped<IFileSystemProvider, InMemoryFileSystemProvider>();
		                    break;
		            }
		        }
		
		        return services;
		    }
		}
	}

	[Fact]
	public async Task Can_Initialize_Wasm()
	{
		var hostingInformation = new LuthetusHostingInformation(
		    LuthetusHostingKind.Wasm,
		    new BackgroundTaskService());
		
		builder.Services.AddLuthetusWebsiteServices(hostingInformation);
	}

	[Fact]
	public async Task Can_Initialize_Photino()
	{
		var hostingInformation = new LuthetusHostingInformation(
            LuthetusHostingKind.Photino,
            new BackgroundTaskService());

        appBuilder.Services.AddLuthetusIdeRazorLibServices(hostingInformation);

		// ... other code

		var continuousStartCts = new CancellationTokenSource();
        var blockingStartCts = new CancellationTokenSource();

        var continuousStopCts = new CancellationTokenSource();
        var blockingStopCts = new CancellationTokenSource();

        var continuousBtw = app.Services.GetRequiredService<ContinuousBackgroundTaskWorker>();
        var blockingBtw = app.Services.GetRequiredService<BlockingBackgroundTaskWorker>();

        var continuousStartTask = continuousBtw.StartAsync(continuousStartCts.Token);
        var blockingStartTask = blockingBtw.StartAsync(blockingStartCts.Token);

        Task continuousStopTask;
        Task blockingStopTask;

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());

            continuousStartCts.Cancel();
            blockingStartCts.Cancel();

            continuousStopTask = continuousBtw.StopAsync(continuousStopCts.Token);
            blockingStopTask = blockingBtw.StopAsync(blockingStopCts.Token);
        };

        AppDomain.CurrentDomain.ProcessExit += (sender, error) =>
        {
            continuousStartCts.Cancel();
            blockingStartCts.Cancel();

            continuousStopCts.Cancel();
            blockingStopCts.Cancel();
        };
	}
}
