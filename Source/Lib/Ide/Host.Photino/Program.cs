using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Dimensions.States;
using Luthetus.Ide.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using System;
using Luthetus.Common.RazorLib.Reflectives.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Luthetus.Ide.Photino;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

        appBuilder.Services.AddLogging();

        var hostingInformation = new LuthetusHostingInformation(
            LuthetusHostingKind.Photino,
            new BackgroundTaskService());

        appBuilder.Services.AddLuthetusIdeRazorLibServices(hostingInformation);

        appBuilder.Services.AddFluxor(options => options.ScanAssemblies(
            typeof(LuthetusCommonConfig).Assembly,
            typeof(LuthetusTextEditorConfig).Assembly,
            typeof(LuthetusIdeConfig).Assembly));

        appBuilder.RootComponents.Add<App>("app");

        var app = appBuilder.Build();

        // customize window
        app.MainWindow
			// Am doing development with a locally published version of the IDE
			// on Ubuntu. The text editor isn't fully optimized,
			// and the default Log for Photino is the console.
			// So, to help the integrated terminal I'm
			// setting verbosity to 0 (which turns off logging) for now (2024-05-14).
			.SetLogVerbosity(0)
            .SetIconFile("favicon.ico")
            .SetTitle("Luthetus IDE")
            .SetDevToolsEnabled(true)
            .SetContextMenuEnabled(true)
            .SetUseOsDefaultSize(false)
            .SetSize(2470, 2000)
            .SetLeft(50)
            .SetTop(50)
			.RegisterSizeChangedHandler((_, size) => 
			{
				var store = app.Services.GetRequiredService<IStore>();
				if (!store.Initialized.IsCompleted)
					return;

				var dispatcher = app.Services.GetRequiredService<IDispatcher>();
				dispatcher.Dispatch(new AppDimensionState.SetAppDimensionStateAction(inState => inState with
				{
					Width = size.Width,
					Height = size.Height
				}));
			})
			.RegisterMaximizedHandler((_, _) =>
			{
				// The 'RegisterSizeChangedHandler' will update the width and height appropriately,
				// when there is a 'maximized' event. But there seems to be a timing issue?
				// Adding this did NOT fix the issue.
				var store = app.Services.GetRequiredService<IStore>();
				if (!store.Initialized.IsCompleted)
					return;

				var dispatcher = app.Services.GetRequiredService<IDispatcher>();
				dispatcher.Dispatch(new AppDimensionState.NotifyIntraAppResizeAction());
			})
			.RegisterRestoredHandler((_, _) =>
			{
				// The 'RegisterSizeChangedHandler' will update the width and height appropriately,
				// when there is a 'restored' event. But there seems to be a timing issue?
				// Adding this did NOT fix the issue.
				var store = app.Services.GetRequiredService<IStore>();
				if (!store.Initialized.IsCompleted)
					return;

				var dispatcher = app.Services.GetRequiredService<IDispatcher>();
				dispatcher.Dispatch(new AppDimensionState.NotifyIntraAppResizeAction());
			});

        // Personal settings to have closing and reopening the IDE be exactly where I want while developing.
        {
            var specialFolderUserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		    if (specialFolderUserProfile == "C:\\Users\\hunte")
                app.MainWindow.SetLeft(1_355);
            else if (specialFolderUserProfile == "/home/hunter")
                app.MainWindow.SetLeft(1_100).SetTop(100).SetHeight(1900);
        }

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

        app.Run();
    }
}
