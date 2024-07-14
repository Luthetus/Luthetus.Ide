using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.Common.RazorLib.TreeViews.Displays.Utils;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.WatchWindows.Displays;
using Luthetus.Common.RazorLib.WatchWindows.Models;

namespace Luthetus.Common.Tests.Basis.TreeViews.Models;

/// <summary>
/// <see cref="TreeViewAdhoc"/>
/// </summary>
public class TreeViewAdhocTests
{
    /// <summary>
    /// <see cref="TreeViewAdhoc(byte)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var item = byte.MinValue;
        var treeViewAdhoc = new TreeViewAdhoc(item);

        Assert.Equal(item, treeViewAdhoc.UntypedItem);
        Assert.Equal(typeof(byte), treeViewAdhoc.ItemType);
        Assert.Null(treeViewAdhoc.Parent);
        Assert.Empty(treeViewAdhoc.ChildList);
        Assert.Equal(0, treeViewAdhoc.IndexAmongSiblings);
        Assert.False(treeViewAdhoc.IsRoot);
        Assert.False(treeViewAdhoc.IsHidden);
        Assert.False(treeViewAdhoc.IsExpandable);
        Assert.True(treeViewAdhoc.IsExpanded);
        Assert.NotEqual(Key<TreeViewChanged>.Empty, treeViewAdhoc.TreeViewChangedKey);
        Assert.NotEqual(Key<TreeViewNoType>.Empty, treeViewAdhoc.Key);
        Assert.Equal(item, treeViewAdhoc.Item);
    }

    /// <summary>
    /// <see cref="TreeViewAdhoc.Equals(object?)"/>
    /// </summary>
    [Fact]
    public void Equals_Test()
    {
        // Same keys, different item
        {
            var key = Key<TreeViewNoType>.NewKey();

            var treeViewAdhocOne = new TreeViewAdhoc(byte.MinValue)
            {
                Key = key
            };

            var treeViewAdhocTwo = new TreeViewAdhoc(byte.MaxValue)
            {
                Key = key
            };

            Assert.Equal(treeViewAdhocOne, treeViewAdhocTwo);
        }

        // Same keys, same item
        {
            var key = Key<TreeViewNoType>.NewKey();
            var item = byte.MinValue;

            var treeViewAdhocOne = new TreeViewAdhoc(item)
            {
                Key = key
            };

            var treeViewAdhocTwo = new TreeViewAdhoc(item)
            {
                Key = key
            };

            Assert.Equal(treeViewAdhocOne, treeViewAdhocTwo);
        }

        // Different keys, different item
        {
            var treeViewAdhocOne = new TreeViewAdhoc(byte.MinValue)
            {
                // The default value is equivalent, but its written here to be explicit
                Key = Key<TreeViewNoType>.NewKey()
            };

            var treeViewAdhocTwo = new TreeViewAdhoc(byte.MaxValue)
            {
                // The default value is equivalent, but its written here to be explicit
                Key = Key<TreeViewNoType>.NewKey()
            };

            Assert.NotEqual(treeViewAdhocOne, treeViewAdhocTwo);
        }
        
        // Different keys, same item
        {
            var item = byte.MinValue;

            var treeViewAdhocOne = new TreeViewAdhoc(item)
            {
                // The default value is equivalent, but its written here to be explicit
                Key = Key<TreeViewNoType>.NewKey()
            };

            var treeViewAdhocTwo = new TreeViewAdhoc(item)
            {
                // The default value is equivalent, but its written here to be explicit
                Key = Key<TreeViewNoType>.NewKey()
            };

            Assert.NotEqual(treeViewAdhocOne, treeViewAdhocTwo);
        }
    }

    /// <summary>
    /// <see cref="TreeViewAdhoc.GetHashCode()"/>
    /// </summary>
    [Fact]
    public void GetHashCode_Test()
    {
        // Same keys, different item
        {
            var key = Key<TreeViewNoType>.NewKey();

            var treeViewAdhocOne = new TreeViewAdhoc(byte.MinValue)
            {
                Key = key
            };

            var treeViewAdhocTwo = new TreeViewAdhoc(byte.MaxValue)
            {
                Key = key
            };

            Assert.Equal(treeViewAdhocOne.GetHashCode(), treeViewAdhocTwo.GetHashCode());
        }

        // Same keys, same item
        {
            var key = Key<TreeViewNoType>.NewKey();
            var item = byte.MinValue;

            var treeViewAdhocOne = new TreeViewAdhoc(item)
            {
                Key = key
            };

            var treeViewAdhocTwo = new TreeViewAdhoc(item)
            {
                Key = key
            };

            Assert.Equal(treeViewAdhocOne.GetHashCode(), treeViewAdhocTwo.GetHashCode());
        }

        // Different keys, different item
        {
            var treeViewAdhocOne = new TreeViewAdhoc(byte.MinValue)
            {
                // The default value is equivalent, but its written here to be explicit
                Key = Key<TreeViewNoType>.NewKey()
            };

            var treeViewAdhocTwo = new TreeViewAdhoc(byte.MaxValue)
            {
                // The default value is equivalent, but its written here to be explicit
                Key = Key<TreeViewNoType>.NewKey()
            };

            Assert.NotEqual(treeViewAdhocOne.GetHashCode(), treeViewAdhocTwo.GetHashCode());
        }

        // Different keys, same item
        {
            var item = byte.MinValue;

            var treeViewAdhocOne = new TreeViewAdhoc(item)
            {
                // The default value is equivalent, but its written here to be explicit
                Key = Key<TreeViewNoType>.NewKey()
            };

            var treeViewAdhocTwo = new TreeViewAdhoc(item)
            {
                // The default value is equivalent, but its written here to be explicit
                Key = Key<TreeViewNoType>.NewKey()
            };

            Assert.NotEqual(treeViewAdhocOne.GetHashCode(), treeViewAdhocTwo.GetHashCode());
        }
    }

    /// <summary>
    /// <see cref="TreeViewAdhoc.ConstructTreeViewAdhoc()"/>
    /// </summary>
    [Fact]
    public void ConstructTreeViewAdhocA()
    {
        var treeViewAdhoc = TreeViewAdhoc.ConstructTreeViewAdhoc();

        Assert.Equal(byte.MinValue, treeViewAdhoc.Item);
        Assert.Null(treeViewAdhoc.Parent);
        Assert.False(treeViewAdhoc.IsExpandable);
        Assert.True(treeViewAdhoc.IsExpanded);
        Assert.Equal(0, treeViewAdhoc.IndexAmongSiblings);
        Assert.True(treeViewAdhoc.IsRoot);
        Assert.True(treeViewAdhoc.IsHidden);
        Assert.NotEqual(Key<TreeViewChanged>.Empty, treeViewAdhoc.TreeViewChangedKey);
    }

    /// <summary>
    /// <see cref="TreeViewAdhoc.ConstructTreeViewAdhoc(TreeViewNoType[])"/>
    /// </summary>
    [Fact]
    public void ConstructTreeViewAdhocB()
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

        var childOneText = "Hello World!";
        var childTwoText = "Goodbye World!";
        var childThreeText = "Salutations World!";

        // 0 children
        {
            var treeViewAdhoc = TreeViewAdhoc.ConstructTreeViewAdhoc(new List<TreeViewNoType>().ToArray());
            AssertTreeViewAdhocIsValid(treeViewAdhoc);

            Assert.Empty(treeViewAdhoc.ChildList);
        }

        // 1 children
        {
            var treeViewAdhoc = TreeViewAdhoc.ConstructTreeViewAdhoc(new List<TreeViewNoType>
            {
                new TreeViewText(childOneText, true, false, commonComponentRenderers),
            }.ToArray());

            AssertTreeViewAdhocIsValid(treeViewAdhoc);

            Assert.Single(treeViewAdhoc.ChildList);

            var childOne = treeViewAdhoc.ChildList[0];
            Assert.Equal(childOneText, (string)childOne.UntypedItem);
        }

        // 2 children
        {
            var treeViewAdhoc = TreeViewAdhoc.ConstructTreeViewAdhoc(new List<TreeViewNoType>
            {
                new TreeViewText(childOneText, true, false, commonComponentRenderers),
                new TreeViewText(childTwoText, true, false, commonComponentRenderers),
            }.ToArray());

            AssertTreeViewAdhocIsValid(treeViewAdhoc);

            Assert.Equal(2, treeViewAdhoc.ChildList.Count);

            var childOne = treeViewAdhoc.ChildList[0];
            Assert.Equal(childOneText, (string)childOne.UntypedItem);

            var childTwo = treeViewAdhoc.ChildList[1];
            Assert.Equal(childTwoText, (string)childTwo.UntypedItem);
        }

        // 3 children
        {
            var treeViewAdhoc = TreeViewAdhoc.ConstructTreeViewAdhoc(new List<TreeViewNoType>
            {
                new TreeViewText(childOneText, true, false, commonComponentRenderers),
                new TreeViewText(childTwoText, true, false, commonComponentRenderers),
                new TreeViewText(childThreeText, true, false, commonComponentRenderers),
            }.ToArray());

            AssertTreeViewAdhocIsValid(treeViewAdhoc);

            Assert.Equal(3, treeViewAdhoc.ChildList.Count);

            var childOne = treeViewAdhoc.ChildList[0];
            Assert.Equal(childOneText, (string)childOne.UntypedItem);

            var childTwo = treeViewAdhoc.ChildList[1];
            Assert.Equal(childTwoText, (string)childTwo.UntypedItem);

            var childThree = treeViewAdhoc.ChildList[2];
            Assert.Equal(childThreeText, (string)childThree.UntypedItem);
        }

        void AssertTreeViewAdhocIsValid(TreeViewAdhoc localTreeViewAdhoc)
        {
            Assert.Equal(byte.MinValue, localTreeViewAdhoc.Item);
            Assert.Null(localTreeViewAdhoc.Parent);
            Assert.False(localTreeViewAdhoc.IsExpandable);
            Assert.True(localTreeViewAdhoc.IsExpanded);
            Assert.Equal(0, localTreeViewAdhoc.IndexAmongSiblings);
            Assert.True(localTreeViewAdhoc.IsRoot);
            Assert.True(localTreeViewAdhoc.IsHidden);
            Assert.NotEqual(Key<TreeViewChanged>.Empty, localTreeViewAdhoc.TreeViewChangedKey);
        }
    }

    /// <summary>
    /// <see cref="TreeViewAdhoc.GetTreeViewRenderer()"/>
    /// </summary>
    [Fact]
    public void GetTreeViewRenderer()
    {
        var treeViewAdhoc = TreeViewAdhoc.ConstructTreeViewAdhoc();

        var treeViewRenderer = treeViewAdhoc.GetTreeViewRenderer();

        Assert.Equal(typeof(TreeViewAdhocDisplay), treeViewRenderer.DynamicComponentType);

        Assert.NotNull(treeViewRenderer.DynamicComponentParameters);
        Assert.Single(treeViewRenderer.DynamicComponentParameters!);

        var keyValuePair = treeViewRenderer.DynamicComponentParameters!.Single();

        Assert.Equal(nameof(TreeViewAdhocDisplay.TreeViewNoTypeAdhoc), keyValuePair.Key);
        Assert.Equal(treeViewAdhoc, keyValuePair.Value);
    }

    /// <summary>
    /// <see cref="TreeViewAdhoc.LoadChildListAsync()"/>
    /// </summary>
    [Fact]
    public async Task LoadChildListAsync()
    {
        var treeViewAdhoc = TreeViewAdhoc.ConstructTreeViewAdhoc();

        Assert.Empty(treeViewAdhoc.ChildList);

        // TreeViewAdhoc.LoadChildListAsync() should do nothing
        await treeViewAdhoc.LoadChildListAsync();
        
        Assert.Empty(treeViewAdhoc.ChildList);
    }

    /// <summary>
    /// <see cref="TreeViewAdhoc.RemoveRelatedFilesFromParent(List{TreeViewNoType})"/>
    /// </summary>
    [Fact]
    public void RemoveRelatedFilesFromParent()
    {
        var treeViewAdhoc = TreeViewAdhoc.ConstructTreeViewAdhoc();

        Assert.Empty(treeViewAdhoc.ChildList);

        // TreeViewAdhoc.RemoveRelatedFilesFromParent(...) should do nothing
        treeViewAdhoc.RemoveRelatedFilesFromParent(new List<TreeViewNoType> { treeViewAdhoc });

        Assert.Empty(treeViewAdhoc.ChildList);
    }
}