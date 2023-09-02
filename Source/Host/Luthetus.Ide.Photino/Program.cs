using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Ide.ClassLib.HostedServiceCase.FileSystem;
using Luthetus.Ide.ClassLib.HostedServiceCase.Terminal;
using Luthetus.Ide.RazorLib;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.TextEditorCase;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using System;
using System.Reflection.PortableExecutable;
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

        appBuilder.Services.AddLuthetusIdeRazorLibServices(options => options with
        {
            IsNativeApplication = true,
        });

        appBuilder.Services.AddLuthetusIdePhotino();

        appBuilder.RootComponents.Add<App>("app");

        var app = appBuilder.Build();

        var backgroundTasksCancellationTokenSource = new CancellationTokenSource();

        var cancellationToken = backgroundTasksCancellationTokenSource.Token;

        InvokeWorkers(app.Services, cancellationToken);

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
            backgroundTasksCancellationTokenSource.Cancel();
            app.MainWindow.ShowMessage("Fatal exception", error.ExceptionObject.ToString());
        };

        app.Run();
    }

    private static void InvokeWorkers(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        var commonQueuedHostedService = serviceProvider.GetRequiredService<LuthetusCommonBackgroundTaskServiceWorker>();
        var textEditorQueuedHostedService = serviceProvider.GetRequiredService<LuthetusTextEditorTextEditorBackgroundTaskServiceWorker>();
        var compilerServiceQueuedHostedService = serviceProvider.GetRequiredService<LuthetusTextEditorCompilerServiceBackgroundTaskServiceWorker>();
        var fileSystemQueuedHostedService = serviceProvider.GetRequiredService<LuthetusIdeFileSystemBackgroundTaskServiceWorker>();
        var terminalQueuedHostedService = serviceProvider.GetRequiredService<LuthetusIdeTerminalBackgroundTaskServiceWorker>();

        _ = Task.Run(async () => await commonQueuedHostedService.StartAsync(cancellationToken));
        _ = Task.Run(async () => await textEditorQueuedHostedService.StartAsync(cancellationToken));
        _ = Task.Run(async () => await compilerServiceQueuedHostedService.StartAsync(cancellationToken));
        _ = Task.Run(async () => await fileSystemQueuedHostedService.StartAsync(cancellationToken));
        _ = Task.Run(async () => await terminalQueuedHostedService.StartAsync(cancellationToken));
    }
}
