using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.Common.RazorLib.UnitTesting;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Luthetus.Ide.Tests.Basics.FileSystem;

/// <summary>Setup the dependency injection necessary</summary>
public class IdeTestingBase
{
    protected readonly ServiceProvider ServiceProvider;
    protected readonly CommonUnitTestHelper CommonHelper;

    public IdeTestingBase()
    {
        var services = new ServiceCollection();

        services.AddScoped<IJSRuntime>(_ => new DoNothingJsRuntime());

        var hostingInformation = new LuthetusHostingInformation(
            LuthetusHostingKind.UnitTesting,
            new BackgroundTaskServiceSynchronous());

        CommonUnitTestHelper.AddLuthetusCommonServicesUnitTesting(services, hostingInformation);

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

        CommonHelper = new CommonUnitTestHelper(ServiceProvider);

        var store = ServiceProvider.GetRequiredService<IStore>();

        store.InitializeAsync().Wait();
    }
}
