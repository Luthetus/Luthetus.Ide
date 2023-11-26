using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.Tests.Basis.WatchWindows;

namespace Luthetus.Common.Tests.Basis.Tabs.Models;

/// <summary>
/// <see cref="TabEntryWithType{T}"/>
/// </summary>
public class TabEntryWithTypeTests
{
    /// <summary>
    /// <see cref="TabEntryWithType{T}.TabEntryWithType(T, Func{TabEntryNoType, string}, Action{TabEntryNoType})"/>
    /// <br/>----<br/>
    /// <see cref="TabEntryWithType{T}.Item"/>
    /// <see cref="TabEntryWithType{T}.UntypedItem"/>
    /// <see cref="TabEntryWithType{T}.ItemType"/>
    /// <see cref="TabEntryWithType{T}.GetDisplayNameFunc"/>
    /// <see cref="TabEntryWithType{T}.OnSetActiveFunc"/>
    /// </summary>
    [Fact]
    public void Constructor()
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

        var tabEntryWithType = new TabEntryWithType<ColorKindTest>(
            item,
            getDisplayNameFunc,
            onSetActiveFunc);

        Assert.Equal(item, tabEntryWithType.Item);
        Assert.Equal(item, tabEntryWithType.UntypedItem);
        Assert.Equal(item.GetType(), tabEntryWithType.ItemType);
        Assert.Equal(getDisplayNameFunc, tabEntryWithType.GetDisplayNameFunc);
        Assert.Equal(onSetActiveFunc, tabEntryWithType.OnSetActiveFunc);

        Assert.NotEqual(Key<TabEntryNoType>.Empty, tabEntryWithType.TabEntryKey);

        // Make another tab entry, then assert that they have different keys
        {
            var otherTabEntryWithType = new TabEntryWithType<ColorKindTest>(
                item,
                getDisplayNameFunc,
                onSetActiveFunc);

            Assert.NotEqual(otherTabEntryWithType.TabEntryKey, tabEntryWithType.TabEntryKey);
        }
    }
}
