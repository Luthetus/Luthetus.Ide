using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Storages.States;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlTypes;

namespace Luthetus.Common.Tests.Basis.Storages.States;

/// <summary>
/// <see cref="StorageSync"/>
/// </summary>
public partial class StorageSyncConstructorTests
{
    /// <summary>
    /// <see cref="StorageSync(IStorageService, IBackgroundTaskService, IDispatcher)"/>
    /// <br/>----<br/>
    /// <see cref="StorageSync.BackgroundTaskService"/>
    /// <see cref="StorageSync.Dispatcher"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        InitializeStorageSyncConstructorTests(
            out var dispatcher,
            out var backgroundTaskService,
            out var storageService);

        var storageSync = new StorageSync(storageService, backgroundTaskService, dispatcher);

        Assert.Equal(dispatcher, storageSync.Dispatcher);
        Assert.Equal(backgroundTaskService, storageSync.BackgroundTaskService);
    }

    private void InitializeStorageSyncConstructorTests(
        out IDispatcher dispatcher,
        out IBackgroundTaskService backgroundTaskService,
        out DoNothingStorageService doNothingStorageService)
    {
        var services = new ServiceCollection()
            .AddScoped<IBackgroundTaskService>(sp => new BackgroundTaskServiceSynchronous())
            .AddScoped<IStorageService, DoNothingStorageService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(StorageState).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        backgroundTaskService = serviceProvider.GetRequiredService<IBackgroundTaskService>();
        doNothingStorageService = (DoNothingStorageService)serviceProvider.GetRequiredService<IStorageService>();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
    }
}