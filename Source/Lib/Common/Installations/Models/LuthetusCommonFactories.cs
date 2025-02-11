using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Drags.Displays;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Common.RazorLib.Installations.Models;

/// <remarks>
/// This class is an exception to the naming convention, "don't use the word 'Luthetus' in class names".
/// 
/// Reason for this exception: when one first starts interacting with this project,
/// 	this type might be one of the first types they interact with. So, the redundancy of namespace
/// 	and type containing 'Luthetus' feels reasonable here.
/// </remarks>
public record LuthetusCommonFactories
{
    public Func<IServiceProvider, IDragService> DragServiceFactory { get; init; } =
        serviceProvider => new DragService();

    public Func<IServiceProvider, IClipboardService> ClipboardServiceFactory { get; init; } =
        serviceProvider => new JavaScriptInteropClipboardService(
            serviceProvider.GetRequiredService<IJSRuntime>());

    public Func<IServiceProvider, IDialogService> DialogServiceFactory { get; init; } =
        serviceProvider => new DialogService(
            serviceProvider.GetRequiredService<IJSRuntime>());

    public Func<IServiceProvider, INotificationService> NotificationServiceFactory { get; init; } =
        serviceProvider => new NotificationService();

    public Func<IServiceProvider, IDropdownService> DropdownServiceFactory { get; init; } =
        serviceProvider => new DropdownService();

    public Func<IServiceProvider, IStorageService> StorageServiceFactory { get; init; } =
        serviceProvider => new LocalStorageService(
            serviceProvider.GetRequiredService<IJSRuntime>());

    public Func<IServiceProvider, IAppOptionsService> AppOptionsServiceFactory { get; init; } =
        serviceProvider => new AppOptionsService(
            serviceProvider.GetRequiredService<IThemeService>(),
            serviceProvider.GetRequiredService<IDispatcher>(),
            serviceProvider.GetRequiredService<IStorageService>(),
            serviceProvider.GetRequiredService<CommonBackgroundTaskApi>(),
            serviceProvider.GetRequiredService<IBackgroundTaskService>());

    public Func<IServiceProvider, IThemeService> ThemeServiceFactory { get; init; } =
        serviceProvider => new ThemeService();

    public Func<IServiceProvider, ITreeViewService> TreeViewServiceFactory { get; init; } =
        serviceProvider => new TreeViewService(serviceProvider.GetRequiredService<IBackgroundTaskService>());

    /// <summary>
    /// The default value for <see cref="EnvironmentProviderFactory"/> is based on the <see cref="LuthetusHostingKind"/>.
    /// Therefore, the uninitialized value is null, then an if statement is performed to determine what it should default to when adding services.
    /// </summary>
    public Func<IServiceProvider, IEnvironmentProvider>? EnvironmentProviderFactory { get; init; }

    /// <summary>
    /// The default value for <see cref="FileSystemProviderFactory"/> is based on the <see cref="LuthetusHostingKind"/>.
    /// Therefore, the uninitialized value is null, then an if statement is performed to determine what it should default to when adding services.
    /// </summary>
    public Func<IServiceProvider, IFileSystemProvider>? FileSystemProviderFactory { get; init; }
}