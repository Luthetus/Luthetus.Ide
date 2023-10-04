using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Tabs.Models;

public record TabGroupLoadTabEntriesParameter(ImmutableList<TabEntryNoType> CurrentTabEntries);
