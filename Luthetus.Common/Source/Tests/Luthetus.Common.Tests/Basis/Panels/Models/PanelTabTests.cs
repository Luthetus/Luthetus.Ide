using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Icons.Displays.Codicon;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Resizes.Displays;
using System.Collections.Immutable;

namespace Luthetus.Common.Tests.Basis.Panels.Models;

/// <summary>
/// <see cref="PanelTab"/>
/// </summary>
public class PanelTabTests
{
    /// <summary>
    /// <see cref="PanelTab(Key{PanelTab}, ElementDimensions, ElementDimensions, Type, Type, string)"/>
    /// <br/>----<br/>
    /// <see cref="PanelTab.IsBeingDragged"/>
    /// <see cref="PanelTab.ContextRecordKey"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var samplePanelGroup = new PanelGroup(
            PanelFacts.LeftPanelRecordKey,
            Key<PanelTab>.Empty,
            new ElementDimensions(),
            ImmutableArray<PanelTab>.Empty);

        var leftPanelGroupWidth = samplePanelGroup.ElementDimensions.DimensionAttributeBag
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

        leftPanelGroupWidth.DimensionUnitBag.AddRange(new[]
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

        var key = Key<PanelTab>.NewKey();
        var elementDimensions = samplePanelGroup.ElementDimensions;
        var beingDraggedDimensions = new ElementDimensions();
        var contentRendererType = typeof(IconCSharpClass);
        var iconRendererType = typeof(IconFolder);
        var displayName = "Solution Explorer";
        var isBeingDragged = false;
        var contextRecordKey = ContextFacts.SolutionExplorerContext.ContextKey;

        var samplePanelTab = new PanelTab(
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

        Assert.Equal(key, samplePanelTab.Key);
        Assert.Equal(elementDimensions, samplePanelTab.ElementDimensions);
        Assert.Equal(beingDraggedDimensions, samplePanelTab.BeingDraggedDimensions);
        Assert.Equal(contentRendererType, samplePanelTab.ContentRendererType);
        Assert.Equal(iconRendererType, samplePanelTab.IconRendererType);
        Assert.Equal(displayName, samplePanelTab.DisplayName);
        Assert.Equal(isBeingDragged, samplePanelTab.IsBeingDragged);
        Assert.Equal(contextRecordKey, samplePanelTab.ContextRecordKey);
    }
}