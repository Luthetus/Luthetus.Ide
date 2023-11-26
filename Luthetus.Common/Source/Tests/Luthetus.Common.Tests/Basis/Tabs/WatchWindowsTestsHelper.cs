using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Tabs.Models;
using System.Collections.Immutable;
using Luthetus.Common.Tests.Basis.Tabs;

namespace Luthetus.Common.Tests.Basis.WatchWindows;

public class TabsTestsHelper
{
    public static void InitializeTabStateActionsTests(
        out TabGroup sampleTabGroup,
        out TabEntryWithType<ColorKindTest> redTabEntry,
        out TabEntryWithType<ColorKindTest> greenTabEntry,
        out TabEntryWithType<ColorKindTest> blueTabEntry,
        out ImmutableList<TabEntryNoType> tabEntries)
    {
        redTabEntry = new TabEntryWithType<ColorKindTest>(
            ColorKindTest.Red,
            tabEntry => ((TabEntryWithType<ColorKindTest>)tabEntry).Item.ToString(),
            _ => { });

        greenTabEntry = new TabEntryWithType<ColorKindTest>(
            ColorKindTest.Green,
            tabEntry => ((TabEntryWithType<ColorKindTest>)tabEntry).Item.ToString(),
            _ => { });

        blueTabEntry = new TabEntryWithType<ColorKindTest>(
            ColorKindTest.Blue,
            tabEntry => ((TabEntryWithType<ColorKindTest>)tabEntry).Item.ToString(),
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
