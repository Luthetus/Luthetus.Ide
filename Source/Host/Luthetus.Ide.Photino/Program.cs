using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.ComponentRunner;
using Luthetus.Common.RazorLib.Installation.Models;
using Luthetus.Ide.RazorLib.InstallationCase.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using System;

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

        appBuilder.Services.AddSingleton(new ComponentRunnerOptions(
            typeof(LuthetusCommonOptions).Assembly,
            typeof(LuthetusTextEditorOptions).Assembly,
            typeof(LuthetusIdeOptions).Assembly));

        appBuilder.RootComponents.Add<App>("app");

        var app = appBuilder.Build();

        // customize window
        app.MainWindow
            .SetIconFile("favicon.ico")
            .SetTitle("Luthetus IDE")
            .SetDevToolsEnabled(true)
            .SetContextMenuEnabled(true)
            .SetUseOsDefaultSize(false)
            .SetSize(2600, 1800)
            .SetLeft(50)
            .SetTop(100);

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
            app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };

        app.Run();
    }
}
