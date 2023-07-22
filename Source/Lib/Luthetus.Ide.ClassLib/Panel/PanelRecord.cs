using Luthetus.Common.RazorLib.Dimensions;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Panel;

public record PanelRecord(
    PanelRecordKey PanelRecordKey,
    PanelTabKey ActivePanelTabKey,
    ElementDimensions ElementDimensions,
    ImmutableArray<PanelTab> PanelTabs);