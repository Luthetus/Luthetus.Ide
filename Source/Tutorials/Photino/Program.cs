using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Tutorials.RazorLib;

namespace Luthetus.Tutorials.Photino;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

        appBuilder.Services.AddLogging();
        
        var hostingInformation = new LuthetusHostingInformation(
            LuthetusHostingKind.Photino,
            LuthetusPurposeKind.TextEditor,
            new BackgroundTaskService());
	        
        appBuilder.Services.AddLuthetusTutorialsRazorLibServices(hostingInformation);

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
            .SetTitle("Project Based Learning")
            .SetDevToolsEnabled(true)
            .SetContextMenuEnabled(true)
            .SetUseOsDefaultSize(false)
            .SetSize(2470, 2000)
            .SetLeft(50)
            .SetTop(50);

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };
        
        hostingInformation.StartBackgroundTaskWorkers(app.Services);
        
        app.Run();
    }
}
