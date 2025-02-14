using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.TreeViews.Displays.Utils;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Microsoft.JSInterop;

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
            .AddScoped<LuthetusCommonApi>(sp =>
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

                return new LuthetusCommonApi(
                    sp.GetRequiredService<IJSRuntime>(),
                    hostingInformation,
                    _commonRendererTypes,
                    hostingInformation.BackgroundTaskService);
            });

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