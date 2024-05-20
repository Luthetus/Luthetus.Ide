using Fluxor;
using Luthetus.Common.RazorLib.Notifications.Models;
using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Luthetus.Common.RazorLib.TreeViews.Displays.Utils;

namespace Luthetus.Common.Tests.Basis.Notifications.Models;

/// <summary>
/// <see cref="NotificationHelper"/>
/// </summary>
public class NotificationHelperTests
{
    /// <summary>
    /// <see cref="NotificationHelper.DispatchInformative(string, string, ILuthetusCommonComponentRenderers, IDispatcher)"/>
    /// </summary>
    [Fact]
    public void DispatchInformative()
    {
        var services = new ServiceCollection()
            .AddScoped<INotificationService, NotificationService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(INotificationService).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();
        
        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        var notificationService = serviceProvider.GetRequiredService<INotificationService>();

        var luthetusCommonTreeViews = new LuthetusCommonTreeViews(
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            typeof(TreeViewTextDisplay),
            typeof(TreeViewReflectionDisplay),
            typeof(TreeViewPropertiesDisplay),
            typeof(TreeViewInterfaceImplementationDisplay),
            typeof(TreeViewFieldsDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewEnumerableDisplay));

        var luthetusCommonComponentRenderers = new LuthetusCommonComponentRenderers(
            typeof(CommonErrorNotificationDisplay),
            typeof(CommonInformativeNotificationDisplay),
            luthetusCommonTreeViews);

        Assert.Empty(notificationService.NotificationStateWrap.Value.DefaultList);

        NotificationHelper.DispatchInformative(
            "Test",
            "Message testing",
            luthetusCommonComponentRenderers,
            dispatcher,
            null);

        Assert.NotEmpty(notificationService.NotificationStateWrap.Value.DefaultList);
    }

    /// <summary>
    /// <see cref="NotificationHelper.DispatchError(string, string, ILuthetusCommonComponentRenderers, IDispatcher)"/>
    /// </summary>
    [Fact]
    public void DispatchError()
    {
        var services = new ServiceCollection()
            .AddScoped<INotificationService, NotificationService>()
            .AddFluxor(options => options.ScanAssemblies(typeof(INotificationService).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

        var notificationService = serviceProvider.GetRequiredService<INotificationService>();

        var luthetusCommonTreeViews = new LuthetusCommonTreeViews(
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            typeof(TreeViewTextDisplay),
            typeof(TreeViewReflectionDisplay),
            typeof(TreeViewPropertiesDisplay),
            typeof(TreeViewInterfaceImplementationDisplay),
            typeof(TreeViewFieldsDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewEnumerableDisplay));

        var luthetusCommonComponentRenderers = new LuthetusCommonComponentRenderers(
            typeof(CommonErrorNotificationDisplay),
            typeof(CommonInformativeNotificationDisplay),
            luthetusCommonTreeViews);

        Assert.Empty(notificationService.NotificationStateWrap.Value.DefaultList);

        NotificationHelper.DispatchError(
            "Test",
            "Message testing",
            luthetusCommonComponentRenderers,
            dispatcher,
            null);

        Assert.NotEmpty(notificationService.NotificationStateWrap.Value.DefaultList);
    }
}