using Fluxor;
using Luthetus.Common.RazorLib;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.WatchWindow.TreeViewDisplays;
using Luthetus.Common.RazorLib.WatchWindow;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Common.Usage.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusCommonUsageServices(
        this IServiceCollection services)
    {
        var watchWindowTreeViewRenderers = new WatchWindowTreeViewRenderers(
            typeof(TreeViewTextDisplay),
            typeof(TreeViewReflectionDisplay),
            typeof(TreeViewPropertiesDisplay),
            typeof(TreeViewInterfaceImplementationDisplay),
            typeof(TreeViewFieldsDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewEnumerableDisplay));

        var commonRendererTypes = new LuthetusCommonComponentRenderers(
            typeof(CommonBackgroundTaskDisplay),
            typeof(CommonErrorNotificationDisplay),
            typeof(CommonInformativeNotificationDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            watchWindowTreeViewRenderers,
            null,
            null);

        // TODO: Move registration of "ILuthetusCommonComponentRenderers" to LuthetusCommon
        services.AddSingleton<ILuthetusCommonComponentRenderers>(_ => commonRendererTypes);

        return services
            .AddLuthetusCommonServices()
            .AddFluxor(options =>
                options.ScanAssemblies(
                    typeof(LuthetusCommonOptions).Assembly));
    }
}
