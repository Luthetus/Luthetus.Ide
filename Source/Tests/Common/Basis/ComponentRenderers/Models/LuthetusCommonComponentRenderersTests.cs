using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.TreeViews.Displays.Utils;
using Luthetus.Common.RazorLib.WatchWindows.Displays;

namespace Luthetus.Common.Tests.Basis.ComponentRenderers.Models;

/// <summary>
/// <see cref="LuthetusCommonComponentRenderers"/>
/// </summary>
public class LuthetusCommonComponentRenderersTests
{
    /// <summary>
    /// <see cref="LuthetusCommonComponentRenderers(Type, Type, LuthetusCommonTreeViews)"/>
    /// <see cref="LuthetusCommonComponentRenderers.LuthetusCommonTreeViews"/>
    /// <see cref="LuthetusCommonComponentRenderers.ErrorNotificationRendererType"/>
    /// <see cref="LuthetusCommonComponentRenderers.InformativeNotificationRendererType"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var commonTreeViews = new CommonTreeViews(
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            typeof(TreeViewTextDisplay),
            typeof(TreeViewReflectionDisplay),
            typeof(TreeViewPropertiesDisplay),
            typeof(TreeViewInterfaceImplementationDisplay),
            typeof(TreeViewFieldsDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewEnumerableDisplay));

        var commonComponentRenderers = new CommonComponentRenderers(
            typeof(CommonErrorNotificationDisplay),
            typeof(CommonInformativeNotificationDisplay),
            typeof(CommonProgressNotificationDisplay),
            commonTreeViews);

        Assert.NotNull(commonComponentRenderers.CommonTreeViews);
        Assert.NotNull(commonComponentRenderers.ErrorNotificationRendererType);
        Assert.NotNull(commonComponentRenderers.ProgressNotificationRendererType);
        Assert.NotNull(commonComponentRenderers.InformativeNotificationRendererType);
    }
}
