using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
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
            .AddScoped<IThemeRecordsCollectionService, ThemeRecordsCollectionService>()
            .AddLuthetusCommonFactories(commonOptions)
            .AddScoped<StorageSync>();

        return services;
    }

    private static IServiceCollection AddLuthetusCommonFactories(
        this IServiceCollection services,
        LuthetusCommonOptions luthetusCommonOptions)
    {
        return services
            .AddScoped(serviceProvider =>
                luthetusCommonOptions.LuthetusCommonFactories.ClipboardServiceFactory.Invoke(serviceProvider))
            .AddScoped(serviceProvider =>
                luthetusCommonOptions.LuthetusCommonFactories.DialogServiceFactory.Invoke(serviceProvider))
            .AddScoped(serviceProvider =>
                luthetusCommonOptions.LuthetusCommonFactories.NotificationServiceFactory.Invoke(serviceProvider))
            .AddScoped(serviceProvider =>
                luthetusCommonOptions.LuthetusCommonFactories.DragServiceFactory.Invoke(serviceProvider))
            .AddScoped(serviceProvider =>
                luthetusCommonOptions.LuthetusCommonFactories.DropdownServiceFactory.Invoke(serviceProvider))
            .AddScoped(serviceProvider =>
                luthetusCommonOptions.LuthetusCommonFactories.AppOptionsServiceFactory.Invoke(serviceProvider))
            .AddScoped(serviceProvider =>
                luthetusCommonOptions.LuthetusCommonFactories.StorageServiceFactory.Invoke(serviceProvider))
            .AddScoped(serviceProvider =>
                luthetusCommonOptions.LuthetusCommonFactories.ThemeServiceFactory.Invoke(serviceProvider))
            .AddScoped(serviceProvider =>
                luthetusCommonOptions.LuthetusCommonFactories.TreeViewServiceFactory.Invoke(serviceProvider)); ;
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