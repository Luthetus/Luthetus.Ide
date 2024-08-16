using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.UnitTesting;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.TextEditor.Tests;

/// <summary>Setup the dependency injection necessary</summary>
public class CommonTestingBase
{
    protected readonly ServiceProvider ServiceProvider;
    protected readonly CommonUnitTestHelper CommonHelper;

    public CommonTestingBase()
    {
        var services = new ServiceCollection();

        var hostingInformation = new LuthetusHostingInformation(
            LuthetusHostingKind.UnitTestingSynchronous,
            LuthetusPurposeKind.Common,
            new BackgroundTaskServiceSynchronous());

        CommonUnitTestHelper.AddLuthetusCommonServicesUnitTesting(services, hostingInformation);

        services.AddFluxor(options => options.ScanAssemblies(
            typeof(LuthetusCommonConfig).Assembly));

        ServiceProvider = services.BuildServiceProvider();

        CommonHelper = new CommonUnitTestHelper(ServiceProvider);

        var store = ServiceProvider.GetRequiredService<IStore>();

        store.InitializeAsync().Wait();
    }
}