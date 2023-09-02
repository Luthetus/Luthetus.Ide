using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Ide.ClassLib.HostedServiceCase.FileSystem;
using Luthetus.Ide.ClassLib.HostedServiceCase.Terminal;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.TextEditorCase;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.TextEditor.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusIdePhotino(
        this IServiceCollection services)
    {
        // The code:
        //     builder.Services.AddHostedService<QueuedHostedService>();
        //
        // is not working for the Photino Blazor app.
        // So manual starting of the service is done.
        return services
            .AddSingleton<LuthetusCommonBackgroundTaskServiceWorker>()
            .AddSingleton<LuthetusTextEditorTextEditorBackgroundTaskServiceWorker>()
            .AddSingleton<LuthetusTextEditorCompilerServiceBackgroundTaskServiceWorker>()
            .AddSingleton<LuthetusIdeFileSystemBackgroundTaskServiceWorker>()
            .AddSingleton<LuthetusIdeTerminalBackgroundTaskServiceWorker>();
    }
}