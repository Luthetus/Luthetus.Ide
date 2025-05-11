using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Widgets.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.TreeViews.Displays.Utils;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Outlines.Models;
using Luthetus.Common.RazorLib.Reflectives.Models;
using Luthetus.Common.RazorLib.Drags.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Themes.Models;

namespace Luthetus.Common.RazorLib.Installations.Models;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// The <see cref="configure"/> parameter provides an instance of a record type.
    /// Use the 'with' keyword to change properties and then return the new instance.
    /// </summary>
    public static IServiceCollection AddLuthetusCommonServices(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation,
        Func<LuthetusCommonConfig, LuthetusCommonConfig>? configure = null)
    {
        var commonConfig = new LuthetusCommonConfig();

        if (configure is not null)
            commonConfig = configure.Invoke(commonConfig);

		var continuousQueue = new BackgroundTaskQueue(
            BackgroundTaskFacts.ContinuousQueueKey,
            "Continuous");
            
        var indefiniteQueue = new BackgroundTaskQueue(
            BackgroundTaskFacts.IndefiniteQueueKey,
            "Blocking");
            
        hostingInformation.BackgroundTaskService.RegisterQueue(continuousQueue);
        hostingInformation.BackgroundTaskService.RegisterQueue(indefiniteQueue);
            
        services
            .AddSingleton(commonConfig)
            .AddSingleton(hostingInformation)
            .AddSingleton<ICommonComponentRenderers>(_ => _commonRendererTypes)
			.AddScoped<BackgroundTaskService>(sp => 
            {
				hostingInformation.BackgroundTaskService.SetContinuousTaskWorker(new ContinuousBackgroundTaskWorker(
				    continuousQueue,
					hostingInformation.BackgroundTaskService,
				    sp.GetRequiredService<ILoggerFactory>(),
				    hostingInformation.LuthetusHostingKind));

				hostingInformation.BackgroundTaskService.SetIndefiniteTaskWorker(new IndefiniteBackgroundTaskWorker(
				    indefiniteQueue,
					hostingInformation.BackgroundTaskService,
				    sp.GetRequiredService<ILoggerFactory>(),
				    hostingInformation.LuthetusHostingKind));

				return hostingInformation.BackgroundTaskService;
			})
            .AddScoped<CommonBackgroundTaskApi>()
            .AddScoped<BrowserResizeInterop>()
            .AddScoped<IContextService, ContextService>()
            .AddScoped<IOutlineService, OutlineService>()
            .AddScoped<IPanelService, PanelService>()
            .AddScoped<IAppDimensionService, AppDimensionService>()
            .AddScoped<IKeymapService, KeymapService>()
            .AddScoped<IWidgetService, WidgetService>()
            .AddScoped<IReflectiveService, ReflectiveService>()
            .AddScoped<IClipboardService, JavaScriptInteropClipboardService>()
            .AddScoped<IDialogService, DialogService>()
            .AddScoped<INotificationService, NotificationService>()
            .AddScoped<IDragService, DragService>()
            .AddScoped<IDropdownService, DropdownService>()
            .AddScoped<IAppOptionsService, AppOptionsService>()
            .AddScoped<IStorageService, LocalStorageService>()
            .AddScoped<IThemeService, ThemeService>()
            .AddScoped<ITreeViewService, TreeViewService>();

        switch (hostingInformation.LuthetusHostingKind)
        {
            case LuthetusHostingKind.Photino:
                services.AddScoped<IEnvironmentProvider, LocalEnvironmentProvider>();
                services.AddScoped<IFileSystemProvider, LocalFileSystemProvider>();
                break;
            default:
                services.AddScoped<IEnvironmentProvider, InMemoryEnvironmentProvider>();
                services.AddScoped<IFileSystemProvider, InMemoryFileSystemProvider>();
                break;
        }
        
        return services;
    }

    private static readonly CommonTreeViews _commonTreeViews = new(
        typeof(TreeViewExceptionDisplay),
        typeof(TreeViewMissingRendererFallbackDisplay),
        typeof(TreeViewTextDisplay),
        typeof(TreeViewReflectionDisplay),
        typeof(TreeViewPropertiesDisplay),
        typeof(TreeViewInterfaceImplementationDisplay),
        typeof(TreeViewFieldsDisplay),
        typeof(TreeViewExceptionDisplay),
        typeof(TreeViewEnumerableDisplay));

    private static readonly CommonComponentRenderers _commonRendererTypes = new(
        typeof(CommonErrorNotificationDisplay),
        typeof(CommonInformativeNotificationDisplay),
        typeof(CommonProgressNotificationDisplay),
        _commonTreeViews);
}