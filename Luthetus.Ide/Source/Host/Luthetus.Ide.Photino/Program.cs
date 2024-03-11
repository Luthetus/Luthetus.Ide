using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
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

        appBuilder.Services.AddSingleton(new ReflectiveOptions(
            typeof(LuthetusCommonConfig).Assembly,
            typeof(LuthetusTextEditorConfig).Assembly,
            typeof(LuthetusIdeConfig).Assembly));

        appBuilder.RootComponents.Add<App>("app");

        var app = appBuilder.Build();

        // customize window
        app.MainWindow
            .SetIconFile("favicon.ico")
            .SetTitle("Luthetus IDE")
            .SetDevToolsEnabled(true)
            .SetContextMenuEnabled(true)
            .SetUseOsDefaultSize(false)
            .SetSize(2600, 2000)
            .SetLeft(50)
            .SetTop(50);

		if (Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) == "C:\\Users\\hunte")
        {
            app.MainWindow.SetLeft(1_200);
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
