using System;
using System.Threading;
using System.Threading.Tasks;
using Luthetus.Common.RazorLib.BackgroundTaskCase;
using Luthetus.Ide.ClassLib.CompilerServices.ParserTaskCase;
using Luthetus.Ide.RazorLib;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;

namespace Luthetus.Ide.Photino
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

            appBuilder.Services
                .AddLogging();

            appBuilder.Services.AddLuthetusIdeRazorLibServices(true);

            // The code:
            //     builder.Services.AddHostedService<QueuedHostedService>();
            //
            // is not working for the Photino Blazor app.
            // So manual starting of the service is done.
            appBuilder.Services.AddSingleton<QueuedHostedService>();
            appBuilder.Services.AddSingleton<ParserHostedService>();

            // register root component and selector
            appBuilder.RootComponents.Add<App>("app");

            var app = appBuilder.Build();

            var backgroundTasksCancellationTokenSource = new CancellationTokenSource();

            var queuedHostedService = app.Services.GetRequiredService<QueuedHostedService>();
            var parserHostedService = app.Services.GetRequiredService<ParserHostedService>();

            var cancellationToken = backgroundTasksCancellationTokenSource.Token;

            _ = Task.Run(async () => await queuedHostedService.StartAsync(cancellationToken));
            _ = Task.Run(async () => await parserHostedService.StartAsync(cancellationToken));

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
    }
}
