using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace Luthetus.Common.Tests.Basis.Installations.Models;

/// <summary>
/// <see cref="ServiceCollectionExtensions"/>
/// </summary>
public class ServiceCollectionExtensionsTests
{
    /// <summary>
    /// <see cref="RazorLib.Installations.Models.ServiceCollectionExtensions.AddLuthetusCommonServices(IServiceCollection, LuthetusHostingInformation, Func{LuthetusCommonConfig, LuthetusCommonConfig}?)"/>
    /// </summary>
    [Fact]
    public void AddLuthetusCommonServices()
    {
        var hostingInformation = new LuthetusHostingInformation(
            LuthetusHostingKind.UnitTesting,
            new BackgroundTaskServiceSynchronous());

        var services = new ServiceCollection()
            .AddLuthetusCommonServices(hostingInformation)
            .AddScoped<ILoggerFactory, NullLoggerFactory>()
            .AddScoped<IJSRuntime>(_ => new DoNothingJsRuntime())
            .AddFluxor(options => options.ScanAssemblies(typeof(LuthetusCommonConfig).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        // 1. configure.Invoke(commonOptions)
        {
            // Goal is to assert that the memory location of the objects don't match
            // when the 'configure' is NOT provided.
            //
            // Then, reinvoke 'AddLuthetusCommonServices' and assert that the memory locations
            // do match, because the 'configure' result was used in place or the default.
            var otherCommonConfig = new LuthetusCommonConfig();

            Assert.False(ReferenceEquals(
                otherCommonConfig,
                serviceProvider.GetRequiredService<LuthetusCommonConfig>()));

            var innerHostingInformation = new LuthetusHostingInformation(
                LuthetusHostingKind.UnitTesting,
                new BackgroundTaskServiceSynchronous());

            var innerServiceCollection = new ServiceCollection()
                .AddLuthetusCommonServices(innerHostingInformation, options => otherCommonConfig)
                .AddScoped<IJSRuntime>(_ => new DoNothingJsRuntime())
                .AddFluxor(options => options.ScanAssemblies(typeof(LuthetusCommonConfig).Assembly));

            var innerServiceProvider = innerServiceCollection.BuildServiceProvider();

            Assert.True(ReferenceEquals(
                otherCommonConfig,
                innerServiceProvider.GetRequiredService<LuthetusCommonConfig>()));
        }

        // 2. RegisterQueue Continuous
        {
            var continuousBackgroundTaskQueueKey = ContinuousBackgroundTaskWorker.GetQueueKey();

            var continuousBackgroundTaskQueue = hostingInformation.BackgroundTaskService.GetQueue(
                continuousBackgroundTaskQueueKey);

            Assert.Equal(continuousBackgroundTaskQueueKey, continuousBackgroundTaskQueue.Key);
        }

        // 3. RegisterQueue Blocking
        {
            var blockingBackgroundTaskQueueKey = BlockingBackgroundTaskWorker.GetQueueKey();

            var blockingBackgroundTaskQueue = hostingInformation.BackgroundTaskService.GetQueue(
                blockingBackgroundTaskQueueKey);

            Assert.Equal(blockingBackgroundTaskQueueKey, blockingBackgroundTaskQueue.Key);
        }

        // 4. ContinuousBackgroundTaskWorker
        {
            Assert.NotNull(serviceProvider.GetRequiredService<ContinuousBackgroundTaskWorker>());
        }

        // 4. BlockingBackgroundTaskWorker
        {
            Assert.NotNull(serviceProvider.GetRequiredService<BlockingBackgroundTaskWorker>());
        }

        // 5. if (ServerSide)
        {
            /*
             {
                AddHostedService(ContinuousBackgroundTaskWorker)
                AddHostedService(BlockingBackgroundTaskWorker)
             }
             */

            // TODO: How should the 'AddHostedService' logic invocations be tested? (2023-11-24)
            // throw new NotImplementedException();
        }

        // 6. commonOptions
        {
            Assert.NotNull(serviceProvider.GetRequiredService<LuthetusCommonConfig>());
        }

        // 7. hostingInformation
        {
            Assert.NotNull(serviceProvider.GetRequiredService<LuthetusHostingInformation>());
        }

        // 8. BackgroundTaskService
        {
            Assert.NotNull(serviceProvider.GetRequiredService<IBackgroundTaskService>());
        }

        // 9. ILuthetusCommonComponentRenderers
        {
            Assert.NotNull(serviceProvider.GetRequiredService<ILuthetusCommonComponentRenderers>());
        }

        // 10. IThemeService
        {
            Assert.NotNull(serviceProvider.GetRequiredService<IThemeService>());
        }

        // 11. AddCommonFactories
        {
            Assert.NotNull(serviceProvider.GetRequiredService<IClipboardService>());
            Assert.NotNull(serviceProvider.GetRequiredService<IDialogService>());
            Assert.NotNull(serviceProvider.GetRequiredService<INotificationService>());
            Assert.NotNull(serviceProvider.GetRequiredService<IDragService>());
            Assert.NotNull(serviceProvider.GetRequiredService<IDropdownService>());
            Assert.NotNull(serviceProvider.GetRequiredService<IAppOptionsService>());
            Assert.NotNull(serviceProvider.GetRequiredService<IStorageService>());
            Assert.NotNull(serviceProvider.GetRequiredService<IThemeService>());
            Assert.NotNull(serviceProvider.GetRequiredService<ITreeViewService>());
            
            Assert.NotNull(serviceProvider.GetRequiredService<IEnvironmentProvider>());
            Assert.NotNull(serviceProvider.GetRequiredService<IFileSystemProvider>());
        }
    }
}