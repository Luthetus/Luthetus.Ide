using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Panels.Models;

public record PanelGroup(
    Key<PanelGroup> Key,
    Key<PanelTab> ActiveTabKey,
    ElementDimensions ElementDimensions,
    ImmutableArray<PanelTab> TabList);