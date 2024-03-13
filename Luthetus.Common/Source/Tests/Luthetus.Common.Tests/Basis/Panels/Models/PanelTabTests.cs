using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
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
            ImmutableArray<Panel>.Empty);

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
        var elementDimensions = samplePanelGroup.ElementDimensions;
        var beingDraggedDimensions = new ElementDimensions();
        var contentRendererType = typeof(IconCSharpClass);
        var iconRendererType = typeof(IconFolder);
        var displayName = "Solution Explorer";
        var isBeingDragged = false;
        var contextRecordKey = ContextFacts.SolutionExplorerContext.ContextKey;

        var samplePanel = new Panel(
            key,
            elementDimensions,
            beingDraggedDimensions,
            // Awkwardly need to provide a type here. Will provide an Icon but this usually
            // would be more along the lines of "typeof(SolutionExplorerDisplay)"
            contentRendererType,
            iconRendererType,
            displayName)
        {
            IsBeingDragged = isBeingDragged,
            ContextRecordKey = contextRecordKey,
        };

        Assert.Equal(key, samplePanel.Key);
        Assert.Equal(elementDimensions, samplePanel.ElementDimensions);
        Assert.Equal(beingDraggedDimensions, samplePanel.BeingDraggedDimensions);
        Assert.Equal(contentRendererType, samplePanel.ContentRendererType);
        Assert.Equal(iconRendererType, samplePanel.IconRendererType);
        Assert.Equal(displayName, samplePanel.DisplayName);
        Assert.Equal(isBeingDragged, samplePanel.IsBeingDragged);
        Assert.Equal(contextRecordKey, samplePanel.ContextRecordKey);
    }
}