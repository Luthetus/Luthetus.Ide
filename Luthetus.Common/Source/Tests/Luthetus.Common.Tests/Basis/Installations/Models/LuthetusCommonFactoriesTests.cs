using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Storages.States;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace Luthetus.Common.Tests.Basis.Installations.Models;

/// <summary>
/// <see cref="LuthetusCommonFactories"/>
/// </summary>
public record LuthetusCommonFactoriesTests
{
    /// <summary>
    /// <see cref="LuthetusCommonOptions.CommonFactories"/>
    /// <br/>----<br/>
    /// <see cref="LuthetusCommonFactories.DragServiceFactory"/>
    /// <see cref="LuthetusCommonFactories.ClipboardServiceFactory"/>
    /// <see cref="LuthetusCommonFactories.DialogServiceFactory"/>
    /// <see cref="LuthetusCommonFactories.NotificationServiceFactory"/>
    /// <see cref="LuthetusCommonFactories.DropdownServiceFactory"/>
    /// <see cref="LuthetusCommonFactories.StorageServiceFactory"/>
    /// <see cref="LuthetusCommonFactories.AppOptionsServiceFactory"/>
    /// <see cref="LuthetusCommonFactories.ThemeServiceFactory"/>
    /// <see cref="LuthetusCommonFactories.TreeViewServiceFactory"/>
    /// </summary>
    [Fact]
    public void CommonFactories()
    {
        var commonOptions = new LuthetusCommonOptions();

        var services = new ServiceCollection()
            .AddScoped(sp => commonOptions.CommonFactories.ClipboardServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.DialogServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.NotificationServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.DragServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.DropdownServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.AppOptionsServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.StorageServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.ThemeServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.TreeViewServiceFactory.Invoke(sp))
            .AddScoped<IJSRuntime>(_ => new DoNothingJsRuntime())
            .AddFluxor(options => options.ScanAssemblies(typeof(LuthetusCommonOptions).Assembly))
            .AddScoped<StorageSync>()
            .AddScoped<IBackgroundTaskService>(sp => new BackgroundTaskServiceSynchronous());

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        Assert.IsType<JavaScriptInteropClipboardService>(serviceProvider.GetRequiredService<IClipboardService>());
        Assert.IsType<DialogService>(serviceProvider.GetRequiredService<IDialogService>());
        Assert.IsType<NotificationService>(serviceProvider.GetRequiredService<INotificationService>());
        Assert.IsType<DragService>(serviceProvider.GetRequiredService<IDragService>());
        Assert.IsType<DropdownService>(serviceProvider.GetRequiredService<IDropdownService>());
        Assert.IsType<AppOptionsService>(serviceProvider.GetRequiredService<IAppOptionsService>());
        Assert.IsType<LocalStorageService>(serviceProvider.GetRequiredService<IStorageService>());
        Assert.IsType<ThemeService>(serviceProvider.GetRequiredService<IThemeService>());
        Assert.IsType<TreeViewService>(serviceProvider.GetRequiredService<ITreeViewService>());
    }

    /// <summary>
    /// <see cref="LuthetusCommonFactories.EnvironmentProviderFactory"/>
    /// </summary>
    [Fact]
    public void EnvironmentProviderFactory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="LuthetusCommonFactories.FileSystemProviderFactory"/>
    /// </summary>
    [Fact]
    public void FileSystemProviderFactory()
    {
        throw new NotImplementedException();
    }
}