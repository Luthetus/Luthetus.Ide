using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.Storages.States;
using Luthetus.Common.RazorLib.Themes.Models;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        Func<LuthetusCommonOptions, LuthetusCommonOptions>? configure = null)
    {
        var commonOptions = new LuthetusCommonOptions();

        if (configure is not null)
            commonOptions = configure.Invoke(commonOptions);

        hostingInformation.BackgroundTaskService.RegisterQueue(ContinuousBackgroundTaskWorker.Queue);
        hostingInformation.BackgroundTaskService.RegisterQueue(BlockingBackgroundTaskWorker.Queue);

        services.AddSingleton(sp => new ContinuousBackgroundTaskWorker(
            ContinuousBackgroundTaskWorker.Queue.Key,
            sp.GetRequiredService<IBackgroundTaskService>(),
            sp.GetRequiredService<ILoggerFactory>()));

        services.AddSingleton(sp => new BlockingBackgroundTaskWorker(
            BlockingBackgroundTaskWorker.Queue.Key,
            sp.GetRequiredService<IBackgroundTaskService>(),
            sp.GetRequiredService<ILoggerFactory>()));

        if (hostingInformation.LuthetusHostingKind == LuthetusHostingKind.ServerSide)
        {
            services.AddHostedService(sp => sp.GetRequiredService<ContinuousBackgroundTaskWorker>());
            services.AddHostedService(sp => sp.GetRequiredService<BlockingBackgroundTaskWorker>());
        }

        services
            .AddSingleton(commonOptions)
            .AddSingleton(hostingInformation)
            .AddSingleton(hostingInformation.BackgroundTaskService)
            .AddSingleton<ILuthetusCommonComponentRenderers>(_ => _commonRendererTypes)
            .AddScoped<IThemeService, ThemeService>()
            .AddCommonFactories(hostingInformation, commonOptions)
            .AddScoped<StorageSync>();

        return services;
    }

    private static IServiceCollection AddCommonFactories(
        this IServiceCollection services,
        LuthetusHostingInformation hostingInformation,
        LuthetusCommonOptions commonOptions)
    {
        services
            .AddScoped(sp => commonOptions.CommonFactories.ClipboardServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.DialogServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.NotificationServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.DragServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.DropdownServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.AppOptionsServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.StorageServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.ThemeServiceFactory.Invoke(sp))
            .AddScoped(sp => commonOptions.CommonFactories.TreeViewServiceFactory.Invoke(sp));

        if (commonOptions.CommonFactories.EnvironmentProviderFactory is not null)
        {
            services.AddScoped(sp => commonOptions.CommonFactories.EnvironmentProviderFactory.Invoke(sp));
        }
        else
        {
            switch (hostingInformation.LuthetusHostingKind)
            {
                case LuthetusHostingKind.Photino:
                    services.AddSingleton<IEnvironmentProvider, LocalEnvironmentProvider>();
                    services.AddSingleton<IFileSystemProvider, LocalFileSystemProvider>();
                    break;
                default:
                    services.AddScoped<IEnvironmentProvider, InMemoryEnvironmentProvider>();
                    services.AddScoped<IFileSystemProvider, InMemoryFileSystemProvider>();
                    break;
            }
        }

        return services;
    }

    private static readonly LuthetusCommonTreeViews _commonTreeViews = new(
        typeof(TreeViewExceptionDisplay),
        typeof(TreeViewMissingRendererFallbackDisplay),
        typeof(TreeViewTextDisplay),
        typeof(TreeViewReflectionDisplay),
        typeof(TreeViewPropertiesDisplay),
        typeof(TreeViewInterfaceImplementationDisplay),
        typeof(TreeViewFieldsDisplay),
        typeof(TreeViewExceptionDisplay),
        typeof(TreeViewEnumerableDisplay));

    private static readonly LuthetusCommonComponentRenderers _commonRendererTypes = new(
        typeof(CommonErrorNotificationDisplay),
        typeof(CommonInformativeNotificationDisplay),
        _commonTreeViews);
}