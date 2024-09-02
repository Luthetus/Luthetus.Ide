using System;
using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Photino.Blazor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Dimensions.States;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.Installations.Models;
using Luthetus.Extensions.Config.Installations.Models;

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
            LuthetusPurposeKind.Ide,
            new BackgroundTaskService());

        appBuilder.Services.AddLuthetusIdeRazorLibServices(hostingInformation);
        appBuilder.Services.AddLuthetusConfigServices(hostingInformation);

        appBuilder.Services.AddFluxor(options => options.ScanAssemblies(
            typeof(LuthetusCommonConfig).Assembly,
            typeof(LuthetusTextEditorConfig).Assembly,
            typeof(LuthetusIdeConfig).Assembly,
            typeof(Luthetus.Extensions.DotNet.Installations.Models.ServiceCollectionExtensions).Assembly));

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

        hostingInformation.GetMainWindowScreenDpiFunc = () =>
        {
            try
            {
                return app.MainWindow.ScreenDpi;
            }
            catch (Exception e)
            {
                // Eat this exception
                return 0;
            }
        };

        // Personal settings to have closing and reopening the IDE be exactly where I want while developing.
        {
            var specialFolderUserProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
		    if (specialFolderUserProfile == "C:\\Users\\hunte")
                app.MainWindow.SetLeft(1_355);
            else if (specialFolderUserProfile == "/home/hunter")
                app.MainWindow.SetLeft(1_200).SetTop(100).SetHeight(1900);
        }

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };
        
        hostingInformation.StartBackgroundTaskWorkers(app.Services);

        app.Run();
    }
}
