using Fluxor;
using Luthetus.Common.RazorLib;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Usage;
using Luthetus.Common.RazorLib.ComponentRenderers;
using Luthetus.Common.RazorLib.Notification;
using Luthetus.Common.RazorLib.WatchWindow.TreeViewDisplays;
using Luthetus.Common.RazorLib.WatchWindow;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.TextEditorCase;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.TextEditor.Installation.RazorLib;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLuthetusTextEditorInstallationServices(
        this IServiceCollection services)
    {
        var watchWindowTreeViewRenderers = new WatchWindowTreeViewRenderers(
            typeof(TreeViewTextDisplay),
            typeof(TreeViewReflectionDisplay),
            typeof(TreeViewPropertiesDisplay),
            typeof(TreeViewInterfaceImplementationDisplay),
            typeof(TreeViewFieldsDisplay),
            typeof(Common.RazorLib.WatchWindow.TreeViewDisplays.TreeViewExceptionDisplay),
            typeof(TreeViewEnumerableDisplay));

        var commonRendererTypes = new LuthetusCommonComponentRenderers(
            typeof(CommonBackgroundTaskDisplay),
            typeof(CommonErrorNotificationDisplay),
            typeof(CommonInformativeNotificationDisplay),
            typeof(Common.RazorLib.WatchWindow.TreeViewDisplays.TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            watchWindowTreeViewRenderers,
            null,
            typeof(CompilerServiceBackgroundTaskDisplay));

        // TODO: Move registration of "ILuthetusCommonComponentRenderers" to LuthetusCommon
        services.AddSingleton<ILuthetusCommonComponentRenderers>(_ => commonRendererTypes);

        services.AddSingleton<ICommonBackgroundTaskQueue, CommonBackgroundTaskQueue>();
        services.AddSingleton<ICommonBackgroundTaskMonitor, CommonBackgroundTaskMonitor>();

        services.AddSingleton<ITextEditorBackgroundTaskQueue, TextEditorBackgroundTaskQueue>();
        services.AddSingleton<ITextEditorBackgroundTaskMonitor, TextEditorBackgroundTaskMonitor>();

        services.AddSingleton<ICompilerServiceBackgroundTaskQueue, CompilerServiceBackgroundTaskQueue>();
        services.AddSingleton<ICompilerServiceBackgroundTaskMonitor, CompilerServiceBackgroundTaskMonitor>();

        services
            .AddLuthetusTextEditor()
            .AddFluxor(options =>
                options.ScanAssemblies(
                    typeof(LuthetusCommonOptions).Assembly,
                    typeof(LuthetusTextEditorOptions).Assembly));

        return services;
    }
}
