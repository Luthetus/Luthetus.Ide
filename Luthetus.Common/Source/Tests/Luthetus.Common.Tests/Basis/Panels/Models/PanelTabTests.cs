using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Icons.Displays.Codicon;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Resizes.Displays;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Panels.Models;

/// <summary>
/// <see cref="Panel"/>
/// </summary>
public class PanelTests
{
    /// <summary>
    /// <see cref="Panel(Key{Panel}, ElementDimensions, ElementDimensions, Type, Type, string)"/>
    /// <br/>----<br/>
    /// <see cref="Panel.IsBeingDragged"/>
    /// <see cref="Panel.ContextRecordKey"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var samplePanelGroup = new PanelGroup(
            PanelFacts.LeftPanelRecordKey,
            Key<Panel>.Empty,
            new ElementDimensions(),
            ImmutableArray<IPanelTab>.Empty);

        var leftPanelGroupWidth = samplePanelGroup.ElementDimensions.DimensionAttributeList
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

        leftPanelGroupWidth.DimensionUnitList.AddRange(new[]
        {
            new DimensionUnit
            {
                Value = 33.3333,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableColumn.RESIZE_HANDLE_WIDTH_IN_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        var key = Key<Panel>.NewKey();
        var dynamicViewModelKey = Key<IDynamicViewModel>.NewKey();
        var beingDraggedDimensions = new ElementDimensions();
        var contentRendererType = typeof(IconCSharpClass);
        var displayName = "Solution Explorer";
        var contextRecordKey = ContextFacts.SolutionExplorerContext.ContextKey;

        var samplePanel = new Panel(
			displayName,
			key,
			dynamicViewModelKey,
			contextRecordKey,
			contentRendererType,
            new());

        Assert.Equal(key, samplePanel.Key);
        Assert.Equal(dynamicViewModelKey, samplePanel.DynamicViewModelKey);
        Assert.Equal(contentRendererType, samplePanel.ComponentType);
        Assert.Equal(displayName, samplePanel.Title);
        Assert.Equal(contextRecordKey, samplePanel.ContextRecordKey);
    }
}