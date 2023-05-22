using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Dimensions;

namespace Luthetus.Ide.ClassLib.Panel;

public record PanelRecord(
    PanelRecordKey PanelRecordKey,
    PanelTabKey ActivePanelTabKey,
    ElementDimensions ElementDimensions,
    ImmutableArray<PanelTab> PanelTabs);