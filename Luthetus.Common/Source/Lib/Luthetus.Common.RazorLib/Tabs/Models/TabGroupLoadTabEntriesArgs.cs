using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Tabs.Models;

public record TabGroupLoadTabEntriesArgs(ImmutableList<TabEntryNoType> CurrentTabEntries);
