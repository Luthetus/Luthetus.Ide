using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Tabs.States;
using static Luthetus.Common.Tests.Basis.Tabs.States.TabStateActionsTests;
using System.Collections.Immutable;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.Tabs.States;

/// <summary>
/// <see cref="TabState"/>
/// </summary>
public class TabStateReducerTests
{
    [Fact]
    public void ReduceRegisterTabGroupAction()
    {
        /*
        [ReducerMethod]
        public static TabState ReduceRegisterTabGroupAction(
            TabState inState, RegisterTabGroupAction registerTabGroupAction)
         */

        InitializeTabStateReducerTests(
            out var dispatcher,
            out var tabStateWrap,
            out var tabGroup,
            out _,
            out _,
            out _,
            out _);

        var registerTabGroupAction = new TabState.RegisterTabGroupAction(tabGroup);

        Assert.DoesNotContain(
            tabStateWrap.Value.TabGroupBag,
            x => x.Key == tabGroup.Key);

        dispatcher.Dispatch(registerTabGroupAction);

        Assert.Contains(
            tabStateWrap.Value.TabGroupBag,
            x => x.Key == tabGroup.Key);
    }

    [Fact]
    public void ReduceDisposeTabGroupAction()
    {
        /*
        [ReducerMethod]
        public static TabState ReduceDisposeTabGroupAction(
            TabState inState, DisposeTabGroupAction disposeTabGroupAction)
         */

        InitializeTabStateReducerTests(
            out var dispatcher,
            out var tabStateWrap,
            out var tabGroup,
            out _,
            out _,
            out _,
            out _);

        var registerTabGroupAction = new TabState.RegisterTabGroupAction(tabGroup);
        dispatcher.Dispatch(registerTabGroupAction);

        Assert.Contains(
            tabStateWrap.Value.TabGroupBag,
            x => x.Key == tabGroup.Key);

        var disposeTabGroupAction = new TabState.DisposeTabGroupAction(tabGroup.Key);
        dispatcher.Dispatch(disposeTabGroupAction);

        Assert.DoesNotContain(
            tabStateWrap.Value.TabGroupBag,
            x => x.Key == tabGroup.Key);
    }

    [Fact]
    public void ReduceSetTabEntryBagAction()
    {
        /*
        [ReducerMethod]
        public static TabState ReduceSetTabEntryBagAction(
            TabState inState, SetTabEntryBagAction setTabEntryBagAction)
         */

        InitializeTabStateReducerTests(
            out var dispatcher,
            out var tabStateWrap,
            out var tabGroup,
            out _,
            out _,
            out _,
            out var tabEntries);

        var registerTabGroupAction = new TabState.RegisterTabGroupAction(tabGroup);
        dispatcher.Dispatch(registerTabGroupAction);

        Assert.Contains(tabStateWrap.Value.TabGroupBag, x => x.Key == tabGroup.Key);

        dispatcher.Dispatch(new TabState.SetTabEntryBagAction(
            tabGroup.Key,
            tabEntries));

        tabGroup = tabStateWrap.Value.TabGroupBag.Single(x => x.Key == tabGroup.Key);

        var emptyTabEntries = ImmutableList<TabEntryNoType>.Empty;
        Assert.NotEqual(emptyTabEntries, tabGroup.EntryBag);

        dispatcher.Dispatch(new TabState.SetTabEntryBagAction(
            tabGroup.Key,
            emptyTabEntries));

        tabGroup = tabStateWrap.Value.TabGroupBag.Single(x => x.Key == tabGroup.Key);
        Assert.Equal(emptyTabEntries, tabGroup.EntryBag);
    }

    [Fact]
    public void ReduceSetActiveTabEntryKeyAction()
    {
        /*
        [ReducerMethod]
        public static TabState ReduceSetActiveTabEntryKeyAction(
            TabState inState, SetActiveTabEntryKeyAction setActiveTabEntryKeyAction)
         */

        InitializeTabStateReducerTests(
            out var dispatcher,
            out var tabStateWrap,
            out var tabGroup,
            out _,
            out _,
            out var blueTabEntry,
            out var tabEntries);

        var registerTabGroupAction = new TabState.RegisterTabGroupAction(tabGroup);
        dispatcher.Dispatch(registerTabGroupAction);

        Assert.Contains(tabStateWrap.Value.TabGroupBag, x => x.Key == tabGroup.Key);

        dispatcher.Dispatch(new TabState.SetTabEntryBagAction(
            tabGroup.Key,
            tabEntries));

        tabGroup = tabStateWrap.Value.TabGroupBag.Single(x => x.Key == tabGroup.Key);

        var emptyTabEntries = ImmutableList<TabEntryNoType>.Empty;
        Assert.NotEqual(emptyTabEntries, tabGroup.EntryBag);

        dispatcher.Dispatch(new TabState.SetActiveTabEntryKeyAction(
            tabGroup.Key,
            blueTabEntry.TabEntryKey));

        tabGroup = tabStateWrap.Value.TabGroupBag.Single(x => x.Key == tabGroup.Key);
        Assert.Equal(blueTabEntry.TabEntryKey, tabGroup.ActiveEntryKey);
    }

    private void InitializeTabStateReducerTests(
        out IDispatcher dispatcher,
        out IState<TabState> tabStateWrap,
        out TabGroup sampleTabGroup,
        out TabEntryWithType<ColorKind> redTabEntry,
        out TabEntryWithType<ColorKind> greenTabEntry,
        out TabEntryWithType<ColorKind> blueTabEntry,
        out ImmutableList<TabEntryNoType> tabEntries)
    {
        var services = new ServiceCollection()
            .AddFluxor(options => options.ScanAssemblies(typeof(TabState).Assembly));

        var serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        tabStateWrap = serviceProvider.GetRequiredService<IState<TabState>>();

        dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

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
