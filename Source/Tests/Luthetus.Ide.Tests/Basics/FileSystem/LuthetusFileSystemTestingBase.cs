using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Luthetus.Ide.Tests.Basics.FileSystem;

/// <summary>
/// Setup the dependency injection necessary
/// </summary>
public class LuthetusFileSystemTestingBase
{
    protected readonly ServiceProvider ServiceProvider;

    protected IEnvironmentProvider EnvironmentProvider => ServiceProvider.GetRequiredService<IEnvironmentProvider>();
    protected IFileSystemProvider FileSystemProvider => ServiceProvider.GetRequiredService<IFileSystemProvider>();
    protected IDispatcher Dispatcher => ServiceProvider.GetRequiredService<IDispatcher>();

    public LuthetusFileSystemTestingBase()
    {
        var services = new ServiceCollection();

        services.AddScoped<IJSRuntime>(_ => new DoNothingJsRuntime());

        var hostingInformation = new LuthetusHostingInformation(
            LuthetusHostingKind.UnitTesting,
            new BackgroundTaskServiceSynchronous());

        services.AddLuthetusCommonServices(hostingInformation, commonOptions =>
        {
            var outLuthetusCommonFactories = commonOptions.LuthetusCommonFactories with
            {
                ClipboardServiceFactory = _ => new InMemoryClipboardService(true),
                StorageServiceFactory = _ => new DoNothingStorageService(true)
            };

            return commonOptions with
            {
                LuthetusCommonFactories = outLuthetusCommonFactories
            };
        });

        services.AddLuthetusTextEditor(hostingInformation, inTextEditorOptions => inTextEditorOptions with
        {
            AddLuthetusCommon = false
        });

        services.AddScoped<IEnvironmentProvider>(_ => new LocalEnvironmentProvider());
        services.AddScoped<IFileSystemProvider>(_ => new LocalFileSystemProvider());

        services.AddFluxor(options => options.ScanAssemblies(
            typeof(LuthetusCommonOptions).Assembly,
            typeof(LuthetusTextEditorOptions).Assembly,
            typeof(RazorLib.Installations.Models.ServiceCollectionExtensions).Assembly));

        ServiceProvider = services.BuildServiceProvider();

        var store = ServiceProvider.GetRequiredService<IStore>();

        store.InitializeAsync().Wait();
    }
}
