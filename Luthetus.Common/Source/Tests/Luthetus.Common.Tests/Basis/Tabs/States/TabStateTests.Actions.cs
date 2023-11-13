using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Tabs.States;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Tabs.States;

/// <summary>
/// <see cref="TabState"/>
/// </summary>
public class TabStateActionsTests
{
    /// <summary>
    /// <see cref="TabState.RegisterTabGroupAction"/>
    /// </summary>
    [Fact]
    public void RegisterTabGroupAction()
    {
        InitializeTabStateActionsTests(
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
        InitializeTabStateActionsTests(
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
        InitializeTabStateActionsTests(
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
        InitializeTabStateActionsTests(
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

    public enum ColorKind
    {
        Red,
        Green,
        Blue,
    }

    private void InitializeTabStateActionsTests(
        out TabGroup sampleTabGroup,
        out TabEntryWithType<ColorKind> redTabEntry,
        out TabEntryWithType<ColorKind> greenTabEntry,
        out TabEntryWithType<ColorKind> blueTabEntry,
        out ImmutableList<TabEntryNoType> tabEntries)
    {
        redTabEntry = new TabEntryWithType<ColorKind>(
            ColorKind.Red,
            tabEntry => ((TabEntryWithType<ColorKind>)tabEntry).Item.ToString(),
            _ => { });

        greenTabEntry = new TabEntryWithType<ColorKind>(
            ColorKind.Green,
            tabEntry => ((TabEntryWithType<ColorKind>)tabEntry).Item.ToString(),
            _ => { });

        blueTabEntry = new TabEntryWithType<ColorKind>(
            ColorKind.Blue,
            tabEntry => ((TabEntryWithType<ColorKind>)tabEntry).Item.ToString(),
            _ => { });

        var temporaryTabEntries = tabEntries = new TabEntryNoType[] 
        { 
            redTabEntry,
            greenTabEntry,
            blueTabEntry,
        }.ToImmutableList();

        sampleTabGroup = new TabGroup(
            loadTabEntriesArgs => Task.FromResult(new TabGroupLoadTabEntriesOutput(temporaryTabEntries)),
            Key<TabGroup>.NewKey());
    }
}
