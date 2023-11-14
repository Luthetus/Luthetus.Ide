using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Resizes.Displays;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Panels.States;

/// <summary>
/// TODO: SphagettiCode - The resizing and hiding/showing is a bit scuffed. (2023-09-19)
/// </summary>
[FeatureState]
public partial record PanelsState(ImmutableArray<PanelGroup> PanelGroupBag)
{
    public PanelsState() : this(ImmutableArray<PanelGroup>.Empty)
    {
        var topLeftGroup = ConstructTopLeftGroup();
        var topRightGroup = ConstructTopRightGroup();
        var bottomGroup = ConstructBottomGroup();

        PanelGroupBag = new[]
        {
            topLeftGroup,
            topRightGroup,
            bottomGroup,
        }.ToImmutableArray();
    }

    public (PanelTab PanelTab, PanelGroup PanelGroup)? DragEventArgs { get; set; }

    private static PanelGroup ConstructTopLeftGroup()
    {
        var leftPanelGroup = new PanelGroup(
            PanelFacts.LeftPanelRecordKey,
            Key<PanelTab>.Empty,
            new ElementDimensions(),
            ImmutableArray<PanelTab>.Empty);

        var leftPanelGroupWidth = leftPanelGroup.ElementDimensions.DimensionAttributeBag
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

        return leftPanelGroup;
    }

    private static PanelGroup ConstructTopRightGroup()
    {
        var rightPanelGroup = new PanelGroup(
            PanelFacts.RightPanelRecordKey,
            Key<PanelTab>.Empty,
            new ElementDimensions(),
            ImmutableArray<PanelTab>.Empty);

        var rightPanelGroupWidth = rightPanelGroup.ElementDimensions.DimensionAttributeBag
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

        rightPanelGroupWidth.DimensionUnitBag.AddRange(new[]
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

        return rightPanelGroup;
    }

    private static PanelGroup ConstructBottomGroup()
    {
        var bottomPanelGroup = new PanelGroup(
            PanelFacts.BottomPanelRecordKey,
            Key<PanelTab>.Empty,
            new ElementDimensions(),
            ImmutableArray<PanelTab>.Empty);

        var bottomPanelGroupHeight = bottomPanelGroup.ElementDimensions.DimensionAttributeBag
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Height);

        bottomPanelGroupHeight.DimensionUnitBag.AddRange(new[]
        {
            new DimensionUnit
            {
                Value = 22,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = ResizableRow.RESIZE_HANDLE_HEIGHT_IN_PIXELS / 2,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            },
            new DimensionUnit
            {
                Value = SizeFacts.Ide.Header.Height.Value / 2,
                DimensionUnitKind = SizeFacts.Ide.Header.Height.DimensionUnitKind,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        return bottomPanelGroup;
    }
}