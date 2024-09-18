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
public partial record PanelState(
	ImmutableArray<PanelGroup> PanelGroupList,
	ImmutableArray<Panel> PanelList)
{
    public PanelState() : this(ImmutableArray<PanelGroup>.Empty, ImmutableArray<Panel>.Empty)
    {
        var topLeftGroup = ConstructTopLeftGroup();
        var topRightGroup = ConstructTopRightGroup();
        var bottomGroup = ConstructBottomGroup();

        PanelGroupList = new[]
        {
            topLeftGroup,
            topRightGroup,
            bottomGroup,
        }.ToImmutableArray();
    }

    public (IPanelTab PanelTab, PanelGroup PanelGroup)? DragEventArgs { get; set; }

    private static PanelGroup ConstructTopLeftGroup()
    {
        var leftPanelGroup = new PanelGroup(
            PanelFacts.LeftPanelGroupKey,
            Key<Panel>.Empty,
            new ElementDimensions(),
            ImmutableArray<IPanelTab>.Empty);

        var leftPanelGroupWidth = leftPanelGroup.ElementDimensions.DimensionAttributeList
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
                Value = 0,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract,
                Purpose = DimensionUnitFacts.Purposes.OFFSET,
            },
        });

        return leftPanelGroup;
    }

    private static PanelGroup ConstructTopRightGroup()
    {
        var rightPanelGroup = new PanelGroup(
            PanelFacts.RightPanelGroupKey,
            Key<Panel>.Empty,
            new ElementDimensions(),
            ImmutableArray<IPanelTab>.Empty);

        var rightPanelGroupWidth = rightPanelGroup.ElementDimensions.DimensionAttributeList
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

        rightPanelGroupWidth.DimensionUnitList.AddRange(new[]
        {
            new DimensionUnit
            {
                Value = 33.3333,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = 0,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract,
                Purpose = DimensionUnitFacts.Purposes.OFFSET,
            },
        });

        return rightPanelGroup;
    }

    private static PanelGroup ConstructBottomGroup()
    {
        var bottomPanelGroup = new PanelGroup(
            PanelFacts.BottomPanelGroupKey,
            Key<Panel>.Empty,
            new ElementDimensions(),
            ImmutableArray<IPanelTab>.Empty);

        var bottomPanelGroupHeight = bottomPanelGroup.ElementDimensions.DimensionAttributeList
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Height);

        bottomPanelGroupHeight.DimensionUnitList.AddRange(new[]
        {
            new DimensionUnit
            {
                Value = 22,
                DimensionUnitKind = DimensionUnitKind.Percentage
            },
            new DimensionUnit
            {
                Value = 0,
                DimensionUnitKind = DimensionUnitKind.Pixels,
                DimensionOperatorKind = DimensionOperatorKind.Subtract,
                Purpose = DimensionUnitFacts.Purposes.OFFSET,
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