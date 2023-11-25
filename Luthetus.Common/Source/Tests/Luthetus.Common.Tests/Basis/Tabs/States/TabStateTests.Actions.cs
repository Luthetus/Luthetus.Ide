using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Tabs.States;
using Luthetus.Common.Tests.Basis.WatchWindows;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Tabs.States;

/// <summary>
/// <see cref="TabState"/>
/// </summary>
public partial class TabStateActionsTests
{
    /// <summary>
    /// <see cref="TabState.RegisterTabGroupAction"/>
    /// </summary>
    [Fact]
    public void RegisterTabGroupAction()
    {
        TabsTestsHelper.InitializeTabStateActionsTests(
            out var tabGroup,
            out _,
            out _,
            out _,
            out _);

        var registerTabGroupAction = new TabState.RegisterTabGroupAction(tabGroup);
        Assert.Equal(tabGroup, registerTabGroupAction.TabGroup);
    }

    /// <summary>
    /// <see cref="TabState.DisposeTabGroupAction"/>
    /// </summary>
    [Fact]
    public void DisposeTabGroupAction()
    {
        TabsTestsHelper.InitializeTabStateActionsTests(
            out var tabGroup,
            out _,
            out _,
            out _,
            out _);

        var disposeTabGroupAction = new TabState.DisposeTabGroupAction(tabGroup.Key);
        Assert.Equal(tabGroup.Key, disposeTabGroupAction.TabGroupKey);
    }

    /// <summary>
    /// <see cref="TabState.SetTabEntryBagAction"/>
    /// </summary>
    [Fact]
    public void SetTabEntryBagAction()
    {
        TabsTestsHelper.InitializeTabStateActionsTests(
            out var tabGroup,
            out _,
            out _,
            out _,
            out _);

        var emptyTabEntries = ImmutableList<TabEntryNoType>.Empty;

        var setTabEntryBagAction = new TabState.SetTabEntryBagAction(
            tabGroup.Key,
            emptyTabEntries);

        Assert.Equal(tabGroup.Key, setTabEntryBagAction.TabGroupKey);
        Assert.Equal(emptyTabEntries, setTabEntryBagAction.TabEntryBag);
    }

    /// <summary>
    /// <see cref="TabState.SetActiveTabEntryKeyAction"/>
    /// </summary>
    [Fact]
    public void SetActiveTabEntryKeyAction()
    {
        TabsTestsHelper.InitializeTabStateActionsTests(
            out var tabGroup,
            out var redTabEntry,
            out _,
            out _,
            out _);

        var setActiveTabEntryKeyAction = new TabState.SetActiveTabEntryKeyAction(
            tabGroup.Key,
            redTabEntry.TabEntryKey);

        Assert.Equal(tabGroup.Key, setActiveTabEntryKeyAction.TabGroupKey);
        Assert.Equal(redTabEntry.TabEntryKey, setActiveTabEntryKeyAction.TabEntryKey);
    }
}
