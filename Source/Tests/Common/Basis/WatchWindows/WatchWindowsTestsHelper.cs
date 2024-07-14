using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.TreeViews.Displays.Utils;
using Luthetus.Common.RazorLib.WatchWindows.Displays;

namespace Luthetus.Common.Tests.Basis.WatchWindows;

public class WatchWindowsTestsHelper
{
    public static void InitializeWatchWindowsTests(
        out PersonTest johnDoe,
        out PersonTest janeDoe,
        out PersonTest bobSmith,
        out CommonComponentRenderers commonComponentRenderers)
    {
        johnDoe = new PersonTest("John", "Doe", new());
        janeDoe = new PersonTest("Jane", "Doe", new());
        bobSmith = new PersonTest("Bob", "Smith", new());

        johnDoe.Relatives.Add(janeDoe);
        janeDoe.Relatives.Add(johnDoe);

        johnDoe.Relatives.Add(bobSmith);
        bobSmith.Relatives.Add(johnDoe);

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

        commonComponentRenderers = new CommonComponentRenderers(
            typeof(CommonErrorNotificationDisplay),
            typeof(CommonInformativeNotificationDisplay),
            typeof(CommonProgressNotificationDisplay),
            commonTreeViews);
    }
}
