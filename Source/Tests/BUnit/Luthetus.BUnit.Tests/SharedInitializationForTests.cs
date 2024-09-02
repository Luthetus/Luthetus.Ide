using System;
using System.Threading;
using System.Threading.Tasks;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.Installations.Models;

namespace Luthetus.BUnit.Tests;

public static class SharedInitializationForTests
{
	public static void Initialize(TestContext ctx)
	{
		var hostingInformation = new LuthetusHostingInformation(
			LuthetusHostingKind.UnitTestingAsync,
			LuthetusPurposeKind.Ide,
			new BackgroundTaskService());
		
		ctx.Services.AddLuthetusIdeRazorLibServices(hostingInformation);
		
		ctx.Services.AddFluxor(options => options.ScanAssemblies(
			typeof(LuthetusCommonConfig).Assembly,
			typeof(LuthetusTextEditorConfig).Assembly,
			typeof(LuthetusIdeConfig).Assembly));

		// Overwrite some services.
		//
		// This code block isn't preferable, but setting these
		// through the 'AddLuthetusIdeRazorLibServices(...)'
		// would be a bit extensive.
		//
		// The IFileSystemProvider, and IEnvironmentProvider however,
		// should not be in here, it is vital that they are reliably
		// NOT using the local filesystem.
		//
		// IClipboardService, and IStorageService on the other hand
		// will likely just throw a JavaScript interop exception
		// in the case that this hack doesn't work.
		{
			ctx.Services.AddScoped<IClipboardService, InMemoryClipboardService>();
			ctx.Services.AddScoped<IStorageService, InMemoryStorageService>();
		}

		var storeInitializer = ctx.RenderComponent<Fluxor.Blazor.Web.StoreInitializer>();
	}

	public static BackgroundTasksHandle StartBackgroundTasks(TestContext ctx)
	{
		return new BackgroundTasksHandle(ctx);
	}

	public class BackgroundTasksHandle
	{
		public BackgroundTasksHandle(TestContext ctx)
		{
			ContinuousBtw = ctx.Services.GetRequiredService<ContinuousBackgroundTaskWorker>();
			ContinuousStartTask = ContinuousBtw.StartAsync(ContinuousStartCts.Token);

			BlockingBtw = ctx.Services.GetRequiredService<BlockingBackgroundTaskWorker>();
			BlockingStartTask = BlockingBtw.StartAsync(BlockingStartCts.Token);

			AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
	        {
	            ContinuousStartCts.Cancel();
	            BlockingStartCts.Cancel();
	
	            ContinuousStopTask = ContinuousBtw.StopAsync(ContinuousStopCts.Token);
	            BlockingStopTask = BlockingBtw.StopAsync(BlockingStopCts.Token);
	        };
	
	        AppDomain.CurrentDomain.ProcessExit += (sender, error) =>
	        {
	            ContinuousStartCts.Cancel();
	            BlockingStartCts.Cancel();
	
	            ContinuousStopCts.Cancel();
	            BlockingStopCts.Cancel();
	        };
		}

		public CancellationTokenSource ContinuousStartCts { get; } = new();
        public CancellationTokenSource ContinuousStopCts { get; } = new();
		public ContinuousBackgroundTaskWorker ContinuousBtw { get; }
        public Task ContinuousStartTask { get; }
		public Task? ContinuousStopTask { get; private set; }

        public CancellationTokenSource BlockingStartCts { get; } = new();
        public CancellationTokenSource BlockingStopCts { get; } = new();
        public BlockingBackgroundTaskWorker BlockingBtw { get; }
        public Task BlockingStartTask { get; }
        public Task? BlockingStopTask { get; private set; }

		public Task Stop()
		{
			ContinuousStopTask = ContinuousBtw.StopAsync(ContinuousStopCts.Token);
	        BlockingStopTask = BlockingBtw.StopAsync(BlockingStopCts.Token);

			return Task.WhenAll(ContinuousStopTask, BlockingStopTask);
		}
	}
}
