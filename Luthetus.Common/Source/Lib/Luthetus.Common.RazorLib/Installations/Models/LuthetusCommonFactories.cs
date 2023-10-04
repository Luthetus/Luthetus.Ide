using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Notifications.States;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Themes.States;
using Luthetus.Common.RazorLib.Options.States;
using Luthetus.Common.RazorLib.TreeViews.States;
using Luthetus.Common.RazorLib.Storages.States;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.RazorLib.Installations.Models;

public record LuthetusCommonFactories
{
    public Func<IServiceProvider, IDragService> DragServiceFactory { get; init; } =
        serviceProvider => new DragService(true);

    public Func<IServiceProvider, IClipboardService> ClipboardServiceFactory { get; init; } =
        serviceProvider => new JavaScriptInteropClipboardService(
            true,
            serviceProvider.GetRequiredService<IJSRuntime>());

    public Func<IServiceProvider, IDialogService> DialogServiceFactory { get; init; } =
        serviceProvider => new DialogService(
            true,
            serviceProvider.GetRequiredService<IDispatcher>(),
            serviceProvider.GetRequiredService<IState<DialogState>>());

    public Func<IServiceProvider, INotificationService> NotificationServiceFactory { get; init; } =
        serviceProvider => new NotificationService(
            true,
            serviceProvider.GetRequiredService<IDispatcher>(),
            serviceProvider.GetRequiredService<IState<NotificationState>>());

    public Func<IServiceProvider, IDropdownService> DropdownServiceFactory { get; init; } =
        serviceProvider => new DropdownService(
            true,
            serviceProvider.GetRequiredService<IDispatcher>(),
            serviceProvider.GetRequiredService<IState<DropdownState>>());

    public Func<IServiceProvider, IStorageService> StorageServiceFactory { get; init; } =
        serviceProvider => new LocalStorageService(
            true,
            serviceProvider.GetRequiredService<IJSRuntime>());

    public Func<IServiceProvider, IAppOptionsService> AppOptionsServiceFactory { get; init; } =
        serviceProvider => new AppOptionsService(
            true,
            serviceProvider.GetRequiredService<IState<AppOptionsState>>(),
            serviceProvider.GetRequiredService<IState<ThemeState>>(),
            serviceProvider.GetRequiredService<IDispatcher>(),
            serviceProvider.GetRequiredService<IStorageService>(),
            serviceProvider.GetRequiredService<StorageSync>());

    public Func<IServiceProvider, IThemeService> ThemeServiceFactory { get; init; } =
        serviceProvider => new ThemeService(
            true,
            serviceProvider.GetRequiredService<IState<ThemeState>>(),
            serviceProvider.GetRequiredService<IDispatcher>());

    public Func<IServiceProvider, ITreeViewService> TreeViewServiceFactory { get; init; } =
        serviceProvider => new TreeViewService(
            true,
            serviceProvider.GetRequiredService<IState<TreeViewState>>(),
            serviceProvider.GetRequiredService<IDispatcher>());
}