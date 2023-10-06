using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.TextEditor.Usage.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusTextEditorUsageServices(
        this IServiceCollection services)
    {
        var luthetusHostingInformation = new LuthetusHostingInformation(
            // LuthetusHostingKind.Wasm,
            // OR
            // LuthetusHostingKind.ServerSide
            new BackgroundTaskService());

        services
            .AddLuthetusTextEditor(luthetusHostingInformation)
            .AddFluxor(options => options.ScanAssemblies(
                typeof(LuthetusCommonOptions).Assembly,
                typeof(LuthetusTextEditorOptions).Assembly));

        return services;
    }
}
