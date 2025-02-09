using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.TreeViews.Displays.Utils;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Luthetus.Common.RazorLib.Dimensions.Models;

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
			.AddCommonFactories(hostingInformation, commonConfig)
			.AddScoped<IBackgroundTaskService>(sp => 
            {
				hostingInformation.BackgroundTaskService.SetContinuousTaskWorker(new BackgroundTaskWorker(
				    continuousQueue,
					hostingInformation.BackgroundTaskService,
				    sp.GetRequiredService<ILoggerFactory>(),
				    hostingInformation.LuthetusHostingKind));

				hostingInformation.BackgroundTaskService.SetIndefiniteTaskWorker(new BackgroundTaskWorker(
				    indefiniteQueue,
					hostingInformation.BackgroundTaskService,
				    sp.GetRequiredService<ILoggerFactory>(),
				    hostingInformation.LuthetusHostingKind));

				return hostingInformation.BackgroundTaskService;
			})
            .AddScoped<CommonBackgroundTaskApi>()
            .AddScoped<BrowserResizeInterop>()
            .AddScoped<IContextService, ContextService>();

        return services;
    }

    private static IServiceCollection AddCommonFactories(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation,
        LuthetusCommonConfig commonConfig)
    {
        services
            .AddScoped(sp => commonConfig.CommonFactories.ClipboardServiceFactory.Invoke(sp))
            .AddScoped(sp => commonConfig.CommonFactories.DialogServiceFactory.Invoke(sp))
            .AddScoped(sp => commonConfig.CommonFactories.NotificationServiceFactory.Invoke(sp))
            .AddScoped(sp => commonConfig.CommonFactories.DragServiceFactory.Invoke(sp))
            .AddScoped(sp => commonConfig.CommonFactories.DropdownServiceFactory.Invoke(sp))
            .AddScoped(sp => commonConfig.CommonFactories.AppOptionsServiceFactory.Invoke(sp))
            .AddScoped(sp => commonConfig.CommonFactories.StorageServiceFactory.Invoke(sp))
            .AddScoped(sp => commonConfig.CommonFactories.ThemeServiceFactory.Invoke(sp))
            .AddScoped(sp => commonConfig.CommonFactories.TreeViewServiceFactory.Invoke(sp));

        if (commonConfig.CommonFactories.EnvironmentProviderFactory is not null &&
            commonConfig.CommonFactories.FileSystemProviderFactory is not null)
        {
            services.AddScoped(sp => commonConfig.CommonFactories.EnvironmentProviderFactory.Invoke(sp));
            services.AddScoped(sp => commonConfig.CommonFactories.FileSystemProviderFactory.Invoke(sp));
        }
        else
        {
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