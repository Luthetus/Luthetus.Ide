using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.WatchWindows.Displays;

namespace Luthetus.Common.Tests.Basis.ComponentRenderers.Models;

/// <summary>
/// <see cref="LuthetusCommonComponentRenderers"/>
/// </summary>
public class LuthetusCommonComponentRenderersTests
{
    /// <summary>
    /// <see cref="LuthetusCommonComponentRenderers(Type, Type, RazorLib.ComponentRenderers.Models.LuthetusCommonTreeViews)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
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
    }

    /// <summary>
    /// <see cref="LuthetusCommonComponentRenderers.ErrorNotificationRendererType"/>
    /// </summary>
    [Fact]
    public void ErrorNotificationRendererType()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="LuthetusCommonComponentRenderers.InformativeNotificationRendererType"/>
    /// </summary>
    [Fact]
    public void InformativeNotificationRendererType()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="LuthetusCommonComponentRenderers.LuthetusCommonTreeViews"/>
    /// </summary>
    [Fact]
    public void LuthetusCommonTreeViews()
    {
        throw new NotImplementedException();
    }
}
