using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.Css;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.JavaScript;
using Luthetus.CompilerServices.Lang.Json;
using Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.TypeScript;
using Luthetus.CompilerServices.Lang.Xml;
using Luthetus.Ide.ClassLib.FileSystem.HostedServiceCase;
using Luthetus.Ide.RazorLib;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.TextEditorCase;
using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Luthetus.Ide.Photino;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

        appBuilder.Services
            .AddLogging();

        appBuilder.Services.AddLuthetusIdeRazorLibServices(true);

        appBuilder.Services.AddScoped<XmlCompilerService>();
        appBuilder.Services.AddScoped<DotNetSolutionCompilerService>();
        appBuilder.Services.AddScoped<CSharpProjectCompilerService>();
        appBuilder.Services.AddScoped<CSharpCompilerService>();
        appBuilder.Services.AddScoped<RazorCompilerService>();
        appBuilder.Services.AddScoped<CssCompilerService>();
        appBuilder.Services.AddScoped<JavaScriptCompilerService>();
        appBuilder.Services.AddScoped<TypeScriptCompilerService>();
        appBuilder.Services.AddScoped<JsonCompilerService>();

        // The code:
        //     builder.Services.AddHostedService<QueuedHostedService>();
        //
        // is not working for the Photino Blazor app.
        // So manual starting of the service is done.
        appBuilder.Services.AddSingleton<CommonQueuedHostedService>();
        appBuilder.Services.AddSingleton<TextEditorQueuedHostedService>();
        appBuilder.Services.AddSingleton<CompilerServiceQueuedHostedService>();
        appBuilder.Services.AddSingleton<FileSystemQueuedHostedService>();

        appBuilder.RootComponents.Add<App>("app");

        var app = appBuilder.Build();

        var backgroundTasksCancellationTokenSource = new CancellationTokenSource();

        var commonQueuedHostedService = app.Services.GetRequiredService<CommonQueuedHostedService>();
        var textEditorQueuedHostedService = app.Services.GetRequiredService<TextEditorQueuedHostedService>();
        var compilerServiceQueuedHostedService = app.Services.GetRequiredService<CompilerServiceQueuedHostedService>();
        var fileSystemQueuedHostedService = app.Services.GetRequiredService<FileSystemQueuedHostedService>();

        var cancellationToken = backgroundTasksCancellationTokenSource.Token;

        _ = Task.Run(async () => await commonQueuedHostedService.StartAsync(cancellationToken));
        _ = Task.Run(async () => await textEditorQueuedHostedService.StartAsync(cancellationToken));
        _ = Task.Run(async () => await compilerServiceQueuedHostedService.StartAsync(cancellationToken));
        _ = Task.Run(async () => await fileSystemQueuedHostedService.StartAsync(cancellationToken));

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
