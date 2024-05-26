using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Luthetus.Common.RazorLib.TreeViews.Displays.Utils;

namespace Luthetus.Common.Tests.Basis.TreeViews.Models;

/// <summary>
/// <see cref="TreeViewNoType"/>
/// </summary>
public class TreeViewNoTypeTests
{
    /// <summary>
    /// <see cref="TreeViewNoType.UntypedItem"/>
    /// <see cref="TreeViewNoType.ItemType"/>
    /// <see cref="TreeViewNoType.Parent"/>
    /// <see cref="TreeViewNoType.ChildList"/>
    /// <see cref="TreeViewNoType.IndexAmongSiblings"/>
    /// <see cref="TreeViewNoType.IsRoot"/>
    /// <see cref="TreeViewNoType.IsHidden"/>
    /// <see cref="TreeViewNoType.IsExpandable"/>
    /// <see cref="TreeViewNoType.IsExpanded"/>
    /// <see cref="TreeViewNoType.TreeViewChangedKey"/>
    /// <see cref="TreeViewNoType.Key"/>
    /// <see cref="TreeViewNoType.GetTreeViewRenderer()"/>
    /// <see cref="TreeViewNoType.LoadChildListAsync()"/>
    /// <see cref="TreeViewNoType.RemoveRelatedFilesFromParent(List{TreeViewNoType})"/>
    /// </summary>
    [Fact]
    public void UntypedItem()
    {
        InitializeTreeViewNoTypeTests(out var commonTreeViews, out var commonComponentRenderers);

        var item = "Hello World!";
        var isExpandable = true;
        var isExpanded = false;
        var treeViewChangedKey = Key<TreeViewChanged>.NewKey();
        var key = Key<TreeViewNoType>.NewKey();

        var treeViewNoType = (TreeViewNoType)new TreeViewText(
            item, isExpandable, isExpanded, commonComponentRenderers)
        {
            TreeViewChangedKey = treeViewChangedKey,
            Key = key,
        };
        
        Assert.Equal(item, treeViewNoType.UntypedItem);
        Assert.Equal(typeof(string), treeViewNoType.ItemType);
        Assert.Null(treeViewNoType.Parent);
        Assert.Equal(new(), treeViewNoType.ChildList);
        Assert.Equal(0, treeViewNoType.IndexAmongSiblings);
        Assert.False(treeViewNoType.IsRoot);
        Assert.False(treeViewNoType.IsHidden);
        Assert.Equal(isExpandable, treeViewNoType.IsExpandable);
        Assert.Equal(isExpanded, treeViewNoType.IsExpanded);
        Assert.Equal(treeViewChangedKey, treeViewNoType.TreeViewChangedKey);
        Assert.Equal(key, treeViewNoType.Key);

        // GetTreeViewRenderer()
        {
            var treeViewRenderer = treeViewNoType.GetTreeViewRenderer();
            Assert.Equal(typeof(TreeViewTextDisplay), treeViewRenderer.DynamicComponentType);

            Assert.NotNull(treeViewRenderer.DynamicComponentParameters);
            var parameter = treeViewRenderer.DynamicComponentParameters!.Single();

            Assert.Equal(nameof(TreeViewText), parameter.Key);
            Assert.Equal(treeViewNoType, parameter.Value);
        }

        // LoadChildListAsync()
        {
            Assert.Empty(treeViewNoType.ChildList);

            treeViewNoType.LoadChildListAsync();
            Assert.Empty(treeViewNoType.ChildList);
        }

        // RemoveRelatedFilesFromParent()
        {
            Assert.Empty(treeViewNoType.ChildList);

            treeViewNoType.RemoveRelatedFilesFromParent(new() { treeViewNoType });
            Assert.Empty(treeViewNoType.ChildList);
        }
    }

    private void InitializeTreeViewNoTypeTests(
        out LuthetusCommonTreeViews commonTreeViews,
        out LuthetusCommonComponentRenderers commonComponentRenderers)
    {
        commonTreeViews = new LuthetusCommonTreeViews(
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewMissingRendererFallbackDisplay),
            typeof(TreeViewTextDisplay),
            typeof(TreeViewReflectionDisplay),
            typeof(TreeViewPropertiesDisplay),
            typeof(TreeViewInterfaceImplementationDisplay),
            typeof(TreeViewFieldsDisplay),
            typeof(TreeViewExceptionDisplay),
            typeof(TreeViewEnumerableDisplay));

        commonComponentRenderers = new LuthetusCommonComponentRenderers(
            typeof(CommonErrorNotificationDisplay),
            typeof(CommonInformativeNotificationDisplay),
            commonTreeViews);
    }
}