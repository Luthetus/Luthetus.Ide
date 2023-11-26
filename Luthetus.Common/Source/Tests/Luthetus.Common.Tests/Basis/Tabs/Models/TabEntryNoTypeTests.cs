using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.Tests.Basis.WatchWindows;

namespace Luthetus.Common.Tests.Basis.Tabs.Models;

/// <summary>
/// <see cref="TabEntryNoType"/>
/// </summary>
public class TabEntryNoTypeTests
{
    /// <summary>
    /// <see cref="TabEntryNoType.UntypedItem"/>
    /// <br/>----<br/>
    /// <see cref="TabEntryNoType.ItemType"/>
    /// <see cref="TabEntryNoType.GetDisplayNameFunc"/>
    /// <see cref="TabEntryNoType.OnSetActiveFunc"/>
    /// <see cref="TabEntryNoType.TabEntryKey"/>
    /// </summary>
    [Fact]
    public void UntypedItem()
    {
        TabsTestsHelper.InitializeTabStateActionsTests(
            out _,
            out _,
            out _,
            out _,
            out _);

        var item = ColorKindTest.Red;

        var getDisplayNameFunc = new Func<TabEntryNoType, string>(
            tabEntry => ((TabEntryWithType<ColorKindTest>)tabEntry).Item.ToString());

        var onSetActiveFunc = new Action<TabEntryNoType>(_ => { });

        var tabEntryNoType = (TabEntryNoType)new TabEntryWithType<ColorKindTest>(
            item,
            getDisplayNameFunc,
            onSetActiveFunc);

        Assert.Equal(item, tabEntryNoType.UntypedItem);
        Assert.Equal(item.GetType(), tabEntryNoType.ItemType);
        Assert.Equal(getDisplayNameFunc, tabEntryNoType.GetDisplayNameFunc);
        Assert.Equal(onSetActiveFunc, tabEntryNoType.OnSetActiveFunc);

        Assert.NotEqual(Key<TabEntryNoType>.Empty, tabEntryNoType.TabEntryKey);

        // Make another tab entry, then assert that they have different keys
        {
            var otherTabEntryNoType = (TabEntryNoType)new TabEntryWithType<ColorKindTest>(
                item,
                getDisplayNameFunc,
                onSetActiveFunc);

            Assert.NotEqual(otherTabEntryNoType.TabEntryKey, tabEntryNoType.TabEntryKey);
        }
    }
}
