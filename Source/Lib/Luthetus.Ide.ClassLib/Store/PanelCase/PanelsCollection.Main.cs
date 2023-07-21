using Luthetus.Ide.ClassLib.Dimensions;
using Luthetus.Ide.ClassLib.Panel;

namespace Luthetus.Ide.ClassLib.Store.PanelCase;

[FeatureState]
public partial record PanelsCollection(
    ImmutableArray<PanelRecord> PanelRecordsList)
{
    public PanelsCollection() : this(
        ImmutableArray<PanelRecord>.Empty)
    {
        var leftPanel = ConstructLeftPanel();

        var rightPanel = ConstructRightPanel();

        var bottomPanel = ConstructBottomPanel();
        
        PanelRecordsList = new[]
        {
            leftPanel,
            rightPanel,
            bottomPanel,
        }.ToImmutableArray();
    }

    public (PanelTab TagDragTarget, PanelRecord ParentPanelRecord)? PanelDragEventArgs { get; set; }
    
    private PanelRecord ConstructLeftPanel()
    {
        var leftPanel = new PanelRecord(
            PanelFacts.LeftPanelRecordKey,
            PanelTabKey.Empty,
            new ElementDimensions(),
            ImmutableArray<PanelTab>.Empty);
        
        var leftPanelWidth = leftPanel.ElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);
        
        leftPanelWidth.DimensionUnits.AddRange(new []
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

        return leftPanel;
    }
    
    private PanelRecord ConstructRightPanel()
    {
        var rightPanel = new PanelRecord(
            PanelFacts.RightPanelRecordKey,
            PanelTabKey.Empty,
            new ElementDimensions(),
            ImmutableArray<PanelTab>.Empty);
        
        var rightPanelWidth = rightPanel.ElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width);
        
        rightPanelWidth.DimensionUnits.AddRange(new []
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
        
        return rightPanel;
    }
    
    private PanelRecord ConstructBottomPanel()
    {
        var bottomPanel = new PanelRecord(
            PanelFacts.BottomPanelRecordKey,
            PanelTabKey.Empty,
            new ElementDimensions(),
            ImmutableArray<PanelTab>.Empty);
        
        var bottomPanelHeight = bottomPanel.ElementDimensions.DimensionAttributes
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Height);
        
        bottomPanelHeight.DimensionUnits.AddRange(new []
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
                Value = SizeFacts.Bstudio.Header.Height.Value / 2,
                DimensionUnitKind = SizeFacts.Bstudio.Header.Height.DimensionUnitKind,
                DimensionOperatorKind = DimensionOperatorKind.Subtract
            }
        });

        return bottomPanel;
    }
}